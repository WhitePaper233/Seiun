using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seiun.Models.Parameters;
using Seiun.Models.Responses;
using Seiun.Resources;
using Seiun.Services;
using Seiun.Utils;
using Seiun.Utils.Enums;

namespace Seiun.Controllers;



[ApiController]
[Route("/admin")]
public class AdminController(ILogger<AdminController> logger, IRepositoryService repository,IJwtService jwt)
    : ControllerBase
{

    [HttpGet("user-list", Name = "GetUserList")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserList()
    {
        try 
        {
            var users = await repository.UserRepository.GetAllAsync();

            var userList = new List<UserList>();
            foreach (var user in users)
            {
                userList.Add(new UserList
                {
                    UserId = user.Id,
                    Role = user.Role,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    NickName = user.NickName,
                    Description = user.Description,
                    LastCheckInTime = await repository.UserCheckInRepository.GetLastCheckInTimeAsync(user.Id) 
                });
            }


            var responseData = new UserListData
            {
                List = [.. userList],
                Total = userList.Count
            };

            return Ok(UserListResp.Success(
                SuccessMessages.Controller.Admin.GetUserListSuccess,
                responseData
            ));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Get user list failed");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                UserListResp.Fail(StatusCodes.Status500InternalServerError,
                ErrorMessages.Controller.Admin.UserListFailed
            ));
        }
    }
    /// <summary>
    /// 管理员登录
    /// </summary>
    /// <param name="userLogin">管理员登录信息DTO</param>
    /// <returns>登录结果DTO</returns>
    [HttpPost("login", Name = "AdminLogin")]
    [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(UserLoginResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UserLoginResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(UserLoginResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
    {
        var user = userLogin switch
        {
            { UserName: { } userName } when !string.IsNullOrWhiteSpace(userName) =>
                await repository.UserRepository.GetByUserNameAsync(userName),
            { PhoneNumber: { } phoneNumber } when !string.IsNullOrWhiteSpace(phoneNumber) =>
                await repository.UserRepository.GetByPhoneNumberAsync(phoneNumber),
            { Email: { } email } when !string.IsNullOrWhiteSpace(email) =>
                await repository.UserRepository.GetByEmailAsync(email),
            _ => null
        };

        if (user == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, UserLoginResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Admin.AdminNotFound
            ));
        }

        if (!PasswordUtils.VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
        {
            return StatusCode(StatusCodes.Status403Forbidden, UserLoginResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Admin.UserLoginFailed
            ));
        }

        var token = jwt.GenerateToken(user);
        var tokenInfo = new TokenInfo
        {
            Token = token,
            ExpireAt = DateTimeOffset.Now.AddHours(Constants.Token.TokenExpirationTime).ToUnixTimeSeconds()
        };
        return Ok(UserLoginResp.Success(SuccessMessages.Controller.User.LoginSuccess, tokenInfo));
    }

    /// <summary>
    /// 管理员更新用户信息、
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="userUpdate">用户更新信息DTO</param>
    /// <returns>更新结果DTO</returns>
    /// <summary>
    [HttpPost("user/update", Name = "UpdateUser")]
    [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateByAdmin userUpdate)
    {
        var user = await repository.UserRepository.GetByIdAsync(userUpdate.UserId);
        if (user == null)
        {
            logger.LogWarning("User not found: {UserId}", userUpdate.UserId);
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.User.UserNotFound
            ));
        }

        user.NickName = string.IsNullOrWhiteSpace(userUpdate.NickName) ? user.NickName : userUpdate.NickName;
        user.Description = string.IsNullOrWhiteSpace(userUpdate.Description) ? user.Description : userUpdate.Description;
        user.Gender = userUpdate.Gender ?? user.Gender;
        repository.UserRepository.Update(user);
        
        if (await repository.UserRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.Admin.UpdateSuccess));
        }

        logger.LogError("User update failed: {UserId}", user.Id);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Admin.ProfileUpdateFailed
        ));
    }

    /// <summary>
    /// 管理员删除用户
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>删除结果DTO</returns>
    [HttpDelete("user/delete/{userId}", Name = "DeleteUser")]
    [Authorize(Roles = $"{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
    {

        var user = await repository.UserRepository.GetByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("User not found: {UserId}", userId);
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.User.UserNotFound
            ));
        }
        repository.UserRepository.Delete(user);
        if (await repository.UserRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.Admin.DeleteSuccess));
        }

        logger.LogError("User delete failed: {UserId}", userId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Admin.ProfileUpdateFailed
        ));
    }
    
}