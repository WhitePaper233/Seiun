using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seiun.Resources;
using Seiun.Models.Responses;
using Seiun.Services;
using Seiun.Utils;
using Seiun.Utils.Enums;
using System.Text.Json;

namespace Seiun.Controllers;

/// <summary>
/// 题目控制器
/// </summary>
/// <param name="repository">日志</param>
[ApiController]
[Route("/api/challenge")]
public class ChallengeController(IRepositoryService repository, ILogger<ChallengeController> logger) : ControllerBase
{
    /// <summary>
    /// 获取题目列表
    /// </summary>
    /// <returns>题目列表</returns>
    [HttpGet("list", Name = "ChallengeList")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(QuestionListResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(QuestionListResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(QuestionListResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChallengeList([FromQuery] ChallengeType challengeType)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, QuestionListResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        List<Guid>? questionList;
        switch (challengeType)
        {
            case ChallengeType.Cloze:
            {
                questionList =
                    (await repository.UserChallengeRepository.GetByUserIdAndQuestionType(userId.Value,
                        ChallengeType.Cloze)).Select(u => u.ChallengeId).ToList();
                break;
            }
            default:
            {
                return BadRequest(QuestionListResp.Fail(
                    StatusCodes.Status400BadRequest,
                    ErrorMessages.Controller.Any.InvalidReqType
                ));
            }
        }
        
        return Ok(QuestionListResp.Success(questionList));
    }

    /// <summary>
    /// 获取选词填空题目
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns>题目</returns>
    [HttpGet("fill-blank", Name = "FillBlank")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FillInBlank([FromQuery] Guid questionId)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, FillInBlankResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var question = await repository.FillInBlankRepository.GetByIdAsync(questionId);
        if (question == null)
            return StatusCode(StatusCodes.Status404NotFound, FillInBlankResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Challenge.QuestionNotFound
            ));

        var questionWord = await repository.FillInBlankWordRepository.GetByQuestionIdAsync(questionId);
        if (questionWord == null || questionWord.Count == 0)
            return StatusCode(StatusCodes.Status404NotFound, FillInBlankResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Challenge.QuestionWordNotFound
            ));

        var questionAnswer = await repository.FillInBlankAnswerRepository.GetByQuestionIdAsync(questionId);
        if (questionAnswer == null || questionAnswer.Count == 0)
            return StatusCode(StatusCodes.Status404NotFound, FillInBlankResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Challenge.QuestionAnswerNotFound
            ));

        var qes = new FillInBlankInfo
        {
            Words = questionWord.Select(a => a.Word).ToList(),
            Content = question.Content,
            Transition = question.Transition,
            Answers = questionAnswer.Select(a =>
                new FillInBlankAnswerInfo
                {
                    Key = a.Key,
                    Answer = a.Answer,
                    Analysis = a.Analysis
                }).ToList()
        };

        return Ok(FillInBlankResp.Success(qes));
    }

    /// <summary>
    /// 获取完形填空题目
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns>题目</returns>
    [HttpGet("cloze-test", Name = "ClozeTest")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ClozeTest([FromQuery] Guid questionId)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, ClozeTestResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var clozeTestEntity = await repository.ClozeTestRepository.GetByIdAsync(questionId);
        if (clozeTestEntity == null)
            return StatusCode(StatusCodes.Status404NotFound, ClozeTestResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Challenge.QuestionNotFound
            ));
    

        var clozeTest = JsonSerializer.Deserialize<ClozeTestDetail>(clozeTestEntity.ClozeTestJson);
        if (clozeTest != null)
        {
            return Ok(ClozeTestResp.Success(clozeTest));
        }
        
        logger.LogWarning("User {} failed get cloze test", userId);
        return StatusCode(StatusCodes.Status500InternalServerError, ClozeTestResp.Fail(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Challenge.GetClozeTestSuccess
        ));
    }
}