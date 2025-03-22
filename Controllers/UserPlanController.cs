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
[Route("/api/user-plan")]
public class UserPlanController(ILogger<UserController> logger, IRepositoryService repository) : ControllerBase
{
    /// <summary>
    /// 获取当前选择的词库
    /// </summary>
    /// <returns>获取结果</returns>
    [HttpGet("current", Name = "GetCurrentWordBook")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(CurrentWordBookResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CurrentWordBookResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(CurrentWordBookResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentWordBook()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, CurrentWordBookResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var currentUserPlan = await repository.UserPlansRepository.GetUserPlanAsync(userId.Value);
        if (currentUserPlan == null)
            return StatusCode(StatusCodes.Status404NotFound, CurrentWordBookResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.UserPlan.CurrentUserPlanNotFound
            ));

        var wordBook = await repository.WordBookRepository.GetByIdAsync(currentUserPlan.WordBookId);
        if (wordBook == null)
            return StatusCode(StatusCodes.Status404NotFound, CurrentWordBookResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.UserPlan.CurrentUserPlanNotFound
            ));

        var wordCount = await repository.WordWordBookRepository.GetBookWordCountAsync(currentUserPlan.WordBookId);
        var remainDays = (wordCount - currentUserPlan.LearnedCount) / currentUserPlan.DailyPlan;

        var currentPlanData = new CurrentPlanData
        {
            WordBookId = wordBook.Id,
            WordBookName = wordBook.WordBookName,
            DailyPlan = currentUserPlan.DailyPlan,
            RemainingDays = remainDays,
            LearnedCount = currentUserPlan.LearnedCount,
            BookWordCount = wordCount,
            ExpectedCompletionAt = DateTimeOffset.UtcNow.AddDays(remainDays).ToUnixTimeSeconds()
        };

        return Ok(CurrentWordBookResp.Success(currentPlanData));
    }

    /// <summary>
    /// 更新计划
    /// </summary>
    /// <param name="updatePlan"></param>
    /// <returns>更新结果</returns>
    [HttpPost("update", Name = "UpdatePlan")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePlan([FromBody] UpdatePlan updatePlan)
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

        userPlanEntity.DailyPlan = updatePlan.DailyPlan;

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