using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    /// <returns>列表</returns>
    [HttpGet("mistake-list", Name = "GetMistakeList")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(MistakeListResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MistakeListResp), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMistakeList()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, MistakeListResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var mistakeList = await repository.MistakeBookRepository.GetByStatus(userId.Value);
        return Ok(MistakeListResp.Success(mistakeList));
    }

    /// <summary>
    /// 获取错题
    /// </summary>
    /// <param name="mistakeWordId">错题ID</param>
    /// <returns>错题</returns>
    [HttpGet("mistake", Name = "GetMistake")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(MistakeDetailResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MistakeDetailResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MistakeDetailResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMistakeDetail([FromQuery] Guid mistakeWordId)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, MistakeDetailResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var mistakeEntity = await repository.WordRepository.GetByIdAsync(mistakeWordId);
        if (mistakeEntity == null)
            return NotFound(MistakeDetailResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.MistakeBook.MistakeWordNotFound
            ));

        return Ok(MistakeDetailResp.Success(mistakeEntity));
    }

    /// <summary>
    /// 获取错题
    /// </summary>
    /// <returns>错题</returns>
    [HttpGet("mistakes", Name = "GetMistakes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(MistakesResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MistakesResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(MistakesResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMistakes()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, MistakesResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var mistakeWordEntities = await repository.MistakeBookRepository.GetMistakeWordDetailsByUserId(userId.Value);
        return Ok(MistakesResp.Success(mistakeWordEntities));
    }
}