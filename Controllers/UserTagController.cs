using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seiun.Entities;
using Seiun.Models.Parameters;
using Seiun.Models.Responses;
using Seiun.Repositories;
using Seiun.Resources;
using Seiun.Services;
using Seiun.Utils;
using Seiun.Utils.Enums;

namespace Seiun.Controllers;

[ApiController]
[Route("/api/user-plan")]
public class UserPlanController(ILogger<UserController> logger, IRepositoryService repository) : ControllerBase
{
    /// <summary>
    /// 获取词库 
    /// </summary>
    /// <returns>获取词库结果</returns>
    [HttpGet("word-bank", Name = "GetWordBank")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(WordBooksResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(WordBooksResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllWordBank()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
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
                    return new WordBook
                    {
                        WordBookId = wordBook.Id,
                        WordBookName = wordBook.WordBookName,
                        WordCount = wordCount,
                        LearnedWordCount = userPlanDict[wordBook.Id].LearnedCount,
                        DailyPlan = userPlanDict[wordBook.Id].SetDailyPlan,
                        RemainingDays = (wordCount - userPlanDict[wordBook.Id].LearnedCount) /
                                        userPlanDict[wordBook.Id].SetDailyPlan
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

            return Ok(WordBooksResp.Success(result));
        }
        catch (Exception e)
        {
            logger.LogError(e, "User {} failed get all word bank", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, WordBooksResp.Fail(
                StatusCodes.Status500InternalServerError,
                ErrorMessages.Controller.UserPlan.GetAllWordBankFailed
            ));
        }
    }

    /// <summary>
    /// 选择词库
    /// </summary>
    /// <param name="selectedWordBank"></param>
    /// <returns>选择结果</returns>
    [HttpPost("select-word-bank", Name = "SelectWordBank")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SelectWordBank([FromBody] UserSelectWordBank selectedWordBank)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var existUserPlan =
            await repository.UserPlansRepository.GetUserPlanAsync(userId.Value,
                selectedWordBank.WordBookId);
        if (existUserPlan == null)
        {
            var userPlanEntity = new UserPlanEntity
            {
                UserId = userId.Value,
                WordBookId = selectedWordBank.WordBookId,
                SetDailyPlan = selectedWordBank.SetDailyPlan,
                LearnedCount = 0
            };
            repository.UserPlansRepository.Create(userPlanEntity);
            if (await repository.UserPlansRepository.SaveAsync())
                return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.UserPlan
                    .SelectWordBankSuccess));

            logger.LogWarning("User {} failed select word bank", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status500InternalServerError,
                ErrorMessages.Controller.UserPlan.SelectWordBankFailed
            ));
        }

        existUserPlan.SetDailyPlan = selectedWordBank.SetDailyPlan;

        repository.UserPlansRepository.Update(existUserPlan);
        if (await repository.UserPlansRepository.SaveAsync())
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.UserPlan
                .SelectWordBankSuccess));

        logger.LogWarning("User {} failed select word bank", userId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.UserPlan.SelectWordBankFailed
        ));
    }

    /// <summary>
    /// 获取当前选择的词库
    /// </summary>
    /// <returns>获取结果</returns>
    [HttpGet("current-word-bank", Name = "GetCurrentWordBank")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(CurrentWordBankResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CurrentWordBankResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(CurrentWordBankResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentWordBank()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, CurrentWordBankResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var currentWordBook = await repository.UserPlansRepository.GetUserPlanAsync(userId.Value);
        if (currentWordBook == null)
            return StatusCode(StatusCodes.Status404NotFound, CurrentWordBankResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.UserPlan.CurrentUserPlanNotFound
            ));

        var wordBook = await repository.WordBookRepository.GetByIdAsync(currentWordBook.WordBookId);
        if (wordBook == null)
            return StatusCode(StatusCodes.Status404NotFound, CurrentWordBankResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.UserPlan.CurrentUserPlanNotFound
            ));

        var remainDays =
            (await repository.WordWordBookRepository.GetBookWordCountAsync(currentWordBook.Id) -
             currentWordBook.LearnedCount) /
            currentWordBook.SetDailyPlan;

        var currentWordBankDetail = new CurrentWordBankDetail
        {
            WordBookId = wordBook.Id,
            WordBookName = wordBook.WordBookName,
            SetDailyPlan = currentWordBook.SetDailyPlan,
            RemainingDays = remainDays,
            LearnedCount = currentWordBook.LearnedCount,
            ExpectedCompletionAt = DateTimeOffset.UtcNow.AddDays(remainDays)
        };

        return Ok(CurrentWordBankResp.Success(currentWordBankDetail));
    }

    /// <summary>
    /// 更新计划
    /// </summary>
    /// <param name="userUpdatePlan"></param>
    /// <returns>更新结果</returns>
    [HttpPost("update-plan", Name = "UpdatePlan")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePlan([FromBody] UserUpdatePlan userUpdatePlan)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var userPlanEntity = await repository.UserPlansRepository.GetUserPlanAsync(userId.Value);
        if (userPlanEntity == null)
            return StatusCode(StatusCodes.Status404NotFound, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.UserPlan.CurrentUserPlanNotFound
            ));

        userPlanEntity.SetDailyPlan = userUpdatePlan.SetDailyPlan;

        repository.UserPlansRepository.Update(userPlanEntity);
        if (await repository.UserPlansRepository.SaveAsync())
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.UserPlan.UpdatePlanSuccess));

        logger.LogWarning("User {} failed update plan", userId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.UserPlan.UpdatePlanFailed
        ));
    }
}