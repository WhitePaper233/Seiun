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
/// <param name="repository">仓库服务</param>
[ApiController]
[Route("/api/mistake-book")]
public class MistakeBookController(IRepositoryService repository)
    : ControllerBase
{
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