using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seiun.Entities;
using Seiun.Models.Parameters;
using Seiun.Models.Responses;
using Seiun.Resources;
using Seiun.Services;
using Seiun.Utils;
using Seiun.Utils.Enums;

namespace Seiun.Controllers;

[ApiController]
[Route("/api/word-book")]
public class WordBookController(ILogger<UserController> logger, IRepositoryService repository) : ControllerBase
{
    /// <summary>
    /// 获取词库 
    /// </summary>
    /// <returns>获取词库结果</returns>
    [HttpGet("books", Name = "GetWordBookList")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(WordBookListResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(WordBookListResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(WordBookListResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWordBookList()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, WordBookListResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        try
        {
            var userPlansOfAllWordBook =
                await repository.UserPlansRepository.GetAllUserPlannedWordBooksAsync(userId.Value);
            var wordBooks = (await repository.WordBookRepository.GetAllAsync()).ToList();

            // 使用 HashSet 提高查询效率
            var userWordBookIds = userPlansOfAllWordBook?.Select(x => x.WordBookId).ToHashSet() ?? [];

            // 获取用户的单词本
            var userWordBooks = wordBooks.Where(u => userWordBookIds.Contains(u.Id)).ToList();

            // 获取其他单词本
            var otherWordBooks = wordBooks.Except(userWordBooks).ToList();

            // 建立一个字典，快速查找 userPlansOfAllWordBook 对应的实体
            var userPlanDict = userPlansOfAllWordBook?.ToDictionary(x => x.WordBookId) ?? [];

            // 生成最终结果
            var result = userWordBooks
                .Where(wordBook => userPlanDict.ContainsKey(wordBook.Id)) // 过滤无效数据
                .Select(wordBook =>
                {
                    var wordCount = repository.WordWordBookRepository.GetBookWordCount(wordBook.Id);
                    var learnedCount = repository.FinishedWordRepository.GetLearnedCount(userId.Value, wordBook.Id);
                    return new WordBook
                    {
                        WordBookId = wordBook.Id,
                        WordBookName = wordBook.WordBookName,
                        WordCount = wordCount,
                        LearnedWordCount = learnedCount,
                        DailyPlan = userPlanDict[wordBook.Id].DailyPlan,
                        RemainingDays = (wordCount - learnedCount) / userPlanDict[wordBook.Id].DailyPlan
                    };
                })
                .ToList();

            // 添加其他单词本
            result.AddRange(otherWordBooks.Select(x => new WordBook
            {
                WordBookId = x.Id,
                WordBookName = x.WordBookName,
                WordCount = repository.WordWordBookRepository.GetBookWordCount(x.Id)
            }));

            return Ok(WordBookListResp.Success(result));
        }
        catch (Exception e)
        {
            logger.LogError(e, "User {} failed get all word books", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, WordBookListResp.Fail(
                StatusCodes.Status500InternalServerError,
                ErrorMessages.Controller.WordBook.GetWordBookListFailed
            ));
        }
    }

    /// <summary>
    /// 选择词库
    /// </summary>
    /// <param name="selectedWordBook"></param>
    /// <returns>选择结果</returns>
    [HttpPost("select", Name = "SelectWordBook")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SelectWordBook([FromBody] SelectWordBook selectedWordBook)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var existUserPlan =
            await repository.UserPlansRepository.GetUserPlanAsync(userId.Value,
                selectedWordBook.WordBookId);
        if (existUserPlan == null)
        {
            var userPlanEntity = new UserPlanEntity
            {
                UserId = userId.Value,
                WordBookId = selectedWordBook.WordBookId,
                DailyPlan = selectedWordBook.DailyPlan
            };
            repository.UserPlansRepository.Create(userPlanEntity);
            if (await repository.UserPlansRepository.SaveAsync())
                return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.WordBook
                    .SelectWordBookSuccess));

            logger.LogWarning("User {} failed select word book", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status500InternalServerError,
                ErrorMessages.Controller.WordBook.SelectWordBookFailed
            ));
        }

        existUserPlan.DailyPlan = selectedWordBook.DailyPlan;

        repository.UserPlansRepository.Update(existUserPlan);
        if (await repository.UserPlansRepository.SaveAsync())
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.WordBook
                .SelectWordBookSuccess));

        logger.LogWarning("User {} failed select word book", userId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.WordBook.SelectWordBookFailed
        ));
    }
}