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
    /// 根据 challengeType 获取用户所有的该类型题目 Id
    /// </summary>
    /// <returns>题目列表</returns>
    [HttpGet("list", Name = "ChallengeList")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(ChallengeListResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ChallengeListResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ChallengeListResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChallengeList([FromQuery] ChallengeType challengeType)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, ChallengeListResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        List<Guid>? challengeIds;
        switch (challengeType)
        {
            case ChallengeType.All:
            {
                challengeIds = await repository.ChallengeRepository.GetByUserId(userId.Value);
                break;
            }
            case ChallengeType.Cloze:
            case ChallengeType.FillInBlank:
            {
                challengeIds = await repository.ChallengeRepository.GetByUserId(userId.Value, challengeType);
                break;
            }
            default:
            {
                return BadRequest(ChallengeListResp.Fail(
                    StatusCodes.Status400BadRequest,
                    ErrorMessages.Controller.Any.InvalidReqType
                ));
            }
        }

        return Ok(ChallengeListResp.Success(challengeIds));
    }

    /// <summary>
    /// 根据 sessionId 获取该会话下的 challengeType 类型题目
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="challengeType"></param>
    /// <returns></returns>
    [HttpGet("session-challenge", Name = "GetSessionChallenge")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetChallengeBySessionId([FromQuery] Guid sessionId,
        [FromQuery] ChallengeType challengeType)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, ClozeTestResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        if (sessionId == Guid.Empty)
            return BadRequest(ClozeTestResp.Fail(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Any.InvalidReqType
            ));

        var userSession = await repository.SessionRepository.GetChallengeByIdAsync(sessionId);
        if (userSession == null)
            return NotFound(ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.WordSession.NotFoundSession
            ));

        switch (challengeType)
        {
            case ChallengeType.FillInBlank:
            {
                var challenge = userSession.Challenges.FirstOrDefault(c => c.Type == challengeType);
                if (challenge == null)
                    return NotFound(ResponseFactory.NewFailedBaseResponse(
                        StatusCodes.Status404NotFound,
                        ErrorMessages.Controller.Challenge.ChallengeNotFound
                    ));

                var fillInBlank = JsonSerializer.Deserialize<FillInBlankDetail>(challenge.ChallengeJson);
                if (fillInBlank == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, FillInBlankResp.Fail(
                        StatusCodes.Status500InternalServerError,
                        ErrorMessages.Controller.Challenge.GetChallengeFailed
                    ));

                return Ok(FillInBlankResp.Success(fillInBlank));
            }
            case ChallengeType.Cloze:
            {
                var challenge = userSession.Challenges.FirstOrDefault(c => c.Type == challengeType);
                if (challenge == null)
                    return NotFound(ResponseFactory.NewFailedBaseResponse(
                        StatusCodes.Status404NotFound,
                        ErrorMessages.Controller.Challenge.ChallengeNotFound
                    ));

                var clozeTest = JsonSerializer.Deserialize<ClozeTestDetail>(challenge.ChallengeJson);
                if (clozeTest == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, ClozeTestResp.Fail(
                        StatusCodes.Status500InternalServerError,
                        ErrorMessages.Controller.Challenge.GetChallengeFailed
                    ));

                return Ok(ClozeTestResp.Success(clozeTest));
            }
            default:
            {
                return BadRequest(ClozeTestResp.Fail(
                    StatusCodes.Status400BadRequest,
                    ErrorMessages.Controller.Any.InvalidReqType
                ));
            }
        }
    }

    /// <summary>
    /// 获取选词填空题目
    /// </summary>
    /// <param name="challengeId"></param>
    /// <returns>题目</returns>
    [HttpGet("fill-blank", Name = "FillBlank")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FillInBlankResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FillInBlank([FromQuery] Guid challengeId)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, FillInBlankResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var fillInBlankEntity = await repository.ChallengeRepository.GetByIdAsync(challengeId);
        if (fillInBlankEntity == null)
            return StatusCode(StatusCodes.Status404NotFound, FillInBlankResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Challenge.ChallengeNotFound
            ));

        var fillInBlank = JsonSerializer.Deserialize<FillInBlankDetail>(fillInBlankEntity.ChallengeJson);
        if (fillInBlank != null) return Ok(FillInBlankResp.Success(fillInBlank));

        logger.LogWarning("User {} failed get fill in blank", userId);
        return StatusCode(StatusCodes.Status500InternalServerError, FillInBlankResp.Fail(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Challenge.GetChallengeSuccess
        ));
    }

    /// <summary>
    /// 获取完形填空题目
    /// </summary>
    /// <param name="challengeId"></param>
    /// <returns>题目</returns>
    [HttpGet("cloze-test", Name = "ClozeTest")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ClozeTestResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ClozeTest([FromQuery] Guid challengeId)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, ClozeTestResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var clozeTestEntity = await repository.ChallengeRepository.GetByIdAsync(challengeId);
        if (clozeTestEntity == null)
            return StatusCode(StatusCodes.Status404NotFound, ClozeTestResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Challenge.ChallengeNotFound
            ));


        var clozeTest = JsonSerializer.Deserialize<ClozeTestDetail>(clozeTestEntity.ChallengeJson);
        if (clozeTest != null) return Ok(ClozeTestResp.Success(clozeTest));

        logger.LogWarning("User {} failed get cloze test", userId);
        return StatusCode(StatusCodes.Status500InternalServerError, ClozeTestResp.Fail(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Challenge.GetChallengeSuccess
        ));
    }
}