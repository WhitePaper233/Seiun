using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Seiun.Services;
using Seiun.Utils;
using Seiun.Resources;
using Seiun.Utils.Enums;
using Seiun.Models.Responses;

namespace Seiun.Controllers;

/// <summary>
///     错题本相关接口
/// </summary>
/// <param name="logger">日志</param>
/// <param name="repository">仓库服务</param>
[ApiController]
[Route("/api/mistake-book")]
public class MistakeBookController(ILogger<MistakeBookController> logger, IRepositoryService repository)
    : ControllerBase
{
    /// <summary>
    /// 获取错题列表
    /// </summary>
    /// <param name="mistakeStatus">筛选条件</param>
    /// <returns>列表</returns>
    [HttpGet("mistake", Name = "GetMistakeBook")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(MistakeListResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MistakeListResp), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMistakeList([FromQuery] MistakeStatus mistakeStatus)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, MistakeListResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var mistakeList = await repository.MistakeBookRepository.GetByStatus(mistakeStatus, userId.Value);
        return Ok(MistakeListResp.Success(mistakeList));
    }

    /// <summary>
    /// 获取错题
    /// </summary>
    /// <param name="mistakeId">错题ID</param>
    /// <returns>错题</returns>
    [HttpGet("mistakes", Name = "GetMistakes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(MistakeDetailResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MistakeDetailResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MistakeDetailResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMistakeDetail([FromQuery] Guid mistakeId)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, MistakeDetailResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var mistakeEntity = await repository.MistakeBookRepository.GetByIdAsync(mistakeId);
        if (mistakeEntity == null)
            return NotFound(MistakeDetailResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.MistakeBook.MistakeNotFound
            ));

        var wordEntity = await repository.WordRepository.GetWordDetailByIdAsync(mistakeEntity.WordId);
        var distractorWordIds = wordEntity.WordDistractors.Select(w => w.DistractorId);
        var distractorWordEntities = (await repository.WordRepository.GetByGuidsAsync(distractorWordIds)).ToList();
        var selectedWordEntity = distractorWordEntities.First(w => w.Id == mistakeEntity.SelectedWordId);

        var options = distractorWordEntities.Select(d =>
                new OptionDetail
                    { WordId = d.Id, Word = d.WordText, Pronunciation = d.Pronunciation, Definition = d.Definition })
            .ToList();
        options.Add(new OptionDetail
        {
            WordId = wordEntity.Id, Word = wordEntity.WordText, Pronunciation = wordEntity.Pronunciation,
            Definition = wordEntity.Definition
        });

        var answer = new AnswerDetail
        {
            WordId = wordEntity.Id,
            Word = wordEntity.WordText
        };

        var selectedWord = new SelectedDetail
        {
            WordId = selectedWordEntity.Id,
            Word = selectedWordEntity.WordText
        };

        var mistake = new MistakeDetail
        {
            Options = options,
            Answer = answer,
            SelectedWord = selectedWord,
            AnswerExampleSentence = wordEntity.ExampleSentence,
            SelectedWordExampleSentence = selectedWordEntity.ExampleSentence,
            Status = mistakeEntity.Status
        };

        return Ok(MistakeDetailResp.Success(mistake));
    }
}