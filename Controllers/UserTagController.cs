using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Seiun.Entities;
using Seiun.Models.Parameters;
using Seiun.Models.Responses;
using Seiun.Resources;
using Seiun.Services;
using Seiun.Utils;
using Seiun.Utils.Enums;

namespace Seiun.Controllers;


[ApiController, Route("/api/tag/user")]
public class TagController(ILogger<UserController> logger, IRepositoryService repository) : ControllerBase
{
    /// <summary>
    /// 获取词库 
    /// </summary>
    /// <returns>获取词库结果</returns>
    [HttpGet("word-bank", Name = "GetWordBank")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(WordBanksResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(WordBanksResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllWordBank()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }

        try
        {
            var userTagsOfAllWordBank = await repository.UserTagRepository.GetUserTagOfAllWordBankAsync(userId.Value);
            var wordBanks = userTagsOfAllWordBank?.Select(u => new WordBank
            {
                WordLevel = u.WordLevel,
                WordCount = u.WordLevel == WordLevel.FourLevel
                    ? Constants.Word.FourLevelWordCount
                    : Constants.Word.SixLevelWordCount,
                LearnedWordCount = u.LearnedCount,
                DailyPlan = u.SetDailyPlan,
                RemainingDays = u.RemainingDays,
                LastStudyAt = u.LastStudyAt
            }).ToList() ?? [];

            var existingLevels = new HashSet<WordLevel>(wordBanks.Select(w => w.WordLevel));
            foreach (var (level, count) in new[]
                     {
                         (WordLevel.FourLevel, Constants.Word.FourLevelWordCount),
                         (WordLevel.SixLevel, Constants.Word.SixLevelWordCount)
                     })
            {
                if (!existingLevels.Contains(level))
                {
                    wordBanks.Add(new WordBank { WordLevel = level, WordCount = count });
                }
            }

            return Ok(WordBanksResp.Success(wordBanks));
        }
        catch (Exception e)
        {
            logger.LogError(e,"User {} failed get all word bank", userId);
            return StatusCode(StatusCodes.Status500InternalServerError,WordBanksResp.Fail(
                StatusCodes.Status500InternalServerError,
                ErrorMessages.Controller.UserTag.GetAllWordBankFailed
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
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SelectWordBank([FromBody] UserSelectWordBank selectedWordBank)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }

        var userExistingTagEntity = await repository.UserTagRepository.GetTagByUserIdAndWordLevelAsync(userId.Value, selectedWordBank.WordLevel);
        if (userExistingTagEntity == null)
        {
            var userTagEntity = new UserTagEntity
            {
                UserId = userId.Value,
                WordLevel = selectedWordBank.WordLevel,
                SetDailyPlan = selectedWordBank.SetDailyPlan,
                SetTotalDays = selectedWordBank.SetTotalDays,
                RemainingDays = selectedWordBank.SetTotalDays,
                LearnedCount = 0,
                ExpectedCompletionAt = selectedWordBank.ExpectedCompletionAt,
                LastStudyAt = null,
                SettingAt = DateTimeOffset.UtcNow
            };
            repository.UserTagRepository.Create(userTagEntity);
            if (await repository.UserTagRepository.SaveAsync())
            {
                return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.UserTag.SelectWordBankSuccess));
            }
            
            logger.LogWarning("User {} failed select word bank", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status500InternalServerError,
                ErrorMessages.Controller.UserTag.SelectWordBankFailed
            ));
        }
        
        userExistingTagEntity.SetDailyPlan = selectedWordBank.SetDailyPlan;
        userExistingTagEntity.SetTotalDays = selectedWordBank.SetTotalDays;
        userExistingTagEntity.RemainingDays = selectedWordBank.SetTotalDays;
        userExistingTagEntity.ExpectedCompletionAt = selectedWordBank.ExpectedCompletionAt;
        userExistingTagEntity.SettingAt = DateTimeOffset.UtcNow;
        
        repository.UserTagRepository.Update(userExistingTagEntity);
        if (await repository.UserTagRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.UserTag.SelectWordBankSuccess));
        }
            
        logger.LogWarning("User {} failed select word bank", userId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.UserTag.SelectWordBankFailed
        ));
    }
    
    /// <summary>
    /// 获取当前选择的词库
    /// </summary>
    /// <returns>获取结果</returns>
    [HttpPost("current-word-bank", Name = "GetCurrentWordBank")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(CurrentWordBankResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CurrentWordBankResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(CurrentWordBankResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentWordBank()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, CurrentWordBankResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }

        var currentWordBank = await repository.UserTagRepository.GetCurrentUserTagAsync(userId.Value);
        if (currentWordBank == null)
        {
            return StatusCode(StatusCodes.Status404NotFound, CurrentWordBankResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.UserTag.CurrentUserTagNotFound
            ));
        }
        // ToDo
        return Ok(currentWordBank);
    }

    /// <summary>
    /// 更新计划
    /// </summary>
    /// <param name="userUpdatePlan"></param>
    /// <returns>更新结果</returns>
    [HttpPost("update-plan", Name = "UpdatePlan")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePlan([FromBody] UserUpdatePlan userUpdatePlan )
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }
        
        var userTagEntity = await repository.UserTagRepository.GetCurrentUserTagAsync(userId.Value);
        if (userTagEntity == null)
        {
            return StatusCode(StatusCodes.Status404NotFound, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.UserTag.CurrentUserTagNotFound
            ));
        }

        userTagEntity.SetDailyPlan = userUpdatePlan.SetDailyPlan;
        userTagEntity.SetTotalDays = userUpdatePlan.SetTotalDays;
        userTagEntity.RemainingDays = userUpdatePlan.SetTotalDays;
        userTagEntity.SettingAt = DateTimeOffset.UtcNow;
        
        repository.UserTagRepository.Update(userTagEntity);
        if (await repository.UserTagRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.UserTag.UpdatePlanSuccess));
        }
        
        logger.LogWarning("User {} failed update plan", userId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.UserTag.UpdatePlanFailed
        ));
    }
}