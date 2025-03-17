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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Seiun.Controllers;

/// <summary>
///     用户相关接口
/// </summary>
/// <param name="logger">日志</param>
/// <param name="repository">仓库服务</param>
/// <param name="jwt">JWT服务</param>
[ApiController]
[Route("/api/user")]
public class UserController(ILogger<UserController> logger, IRepositoryService repository, IJwtService jwt)
    : ControllerBase
{
    /// <summary>
    ///     用户注册
    /// </summary>
    /// <param name="userRegister">用户注册信息DTO</param>
    /// <returns>注册结果DTO</returns>
    [HttpPost("register", Name = "Register")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] UserRegister userRegister)
    {
        var user = await repository.UserRepository.GetByPhoneNumberAsync(userRegister.PhoneNumber);
        if (user != null)
            return Conflict(ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status409Conflict,
                ErrorMessages.Controller.User.PhoneNumberDuplicated
            ));

        var (passwordHash, passwordSalt) = PasswordUtils.CreatePasswordHash(userRegister.Password);
        user = new UserEntity
        {
            UserName = $"User_{userRegister.PhoneNumber}",
            NickName = RandomUtils.GenerateRandomString(Constants.User.MaxNickNameLength),
            PhoneNumber = userRegister.PhoneNumber,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = UserRole.User,
            Gender = Gender.Unknown,
            CreatedAt = DateTimeOffset.UtcNow,
            IsBanned = false
        };

        repository.UserRepository.Create(user);
        if (await repository.UserRepository.SaveAsync())
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.User.RegisterSuccess));

        logger.LogError("User {} register failed", user.PhoneNumber);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.User.RegisterFailed
        ));
    }

    /// <summary>
    ///     用户登录
    /// </summary>
    /// <param name="userLogin">用户登录信息DTO</param>
    /// <returns>登录结果DTO</returns>
    [HttpPost("login", Name = "Login")]
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
            return StatusCode(StatusCodes.Status403Forbidden, UserLoginResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.User.UserNotFound
            ));

        if (!PasswordUtils.VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
            return StatusCode(StatusCodes.Status403Forbidden, UserLoginResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.User.UserLoginFailed
            ));

        var token = jwt.GenerateToken(user);
        var tokenInfo = new TokenInfo
        {
            Token = token,
            UserId = user.Id.ToString(),
            ExpireAt = DateTimeOffset.Now.AddHours(Constants.Token.TokenExpirationTime).ToUnixTimeSeconds()
            // ExpireAt = DateTimeOffset.Now.AddSeconds(60).ToUnixTimeSeconds() // only test code
        };
        return Ok(UserLoginResp.Success(tokenInfo));
    }

    /// <summary>
    ///     续签 JWT Token
    /// </summary>
    /// <returns>新 Token</returns>
    [HttpGet("refresh-token", Name = "RefreshToken")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RefreshToken()
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, TokenRefreshResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var user = await repository.UserRepository.GetByIdAsync(userId.Value);
        if (user == null)
            return StatusCode(StatusCodes.Status403Forbidden, TokenRefreshResp.Fail(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.User.UserNotFound
            ));

        var token = jwt.GenerateToken(user);
        var tokenInfo = new TokenInfo
        {
            Token = token,
            UserId = user.Id.ToString(),
            ExpireAt = DateTimeOffset.Now.AddHours(Constants.Token.TokenExpirationTime).ToUnixTimeSeconds()
            // ExpireAt = DateTimeOffset.Now.AddSeconds(60).ToUnixTimeSeconds() // only test code
        };
        return Ok(TokenRefreshResp.Success(tokenInfo));
    }


    /// <summary>
    ///     更新用户信息
    /// </summary>
    /// <param name="userUpdateProfile">更新信息DTO</param>
    /// <returns>更新结果</returns>
    [HttpPatch("update-profile", Name = "UpdateProfile")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateProfile userUpdateProfile)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var user = await repository.UserRepository.GetByIdAsync(userId.Value);
        if (user == null)
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.User.UserNotFound
            ));

        user.NickName = string.IsNullOrWhiteSpace(userUpdateProfile.NickName)
            ? user.NickName
            : userUpdateProfile.NickName;
        user.Description = string.IsNullOrWhiteSpace(userUpdateProfile.Description)
            ? user.Description
            : userUpdateProfile.Description;
        user.Gender = userUpdateProfile.Gender ?? user.Gender;

        repository.UserRepository.Update(user);
        if (!await repository.UserRepository.SaveAsync())
            return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status500InternalServerError,
                ErrorMessages.Controller.User.ProfileUpdateFailed
            ));

        return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.User.ProfileUpdateSuccess));
    }

    /// <summary>
    ///     更新用户头像
    /// </summary>
    /// <param name="avatarFile">头像文件</param>
    /// <returns>更新结果</returns>
    [HttpPost("upload-avatar", Name = "UploadAvatar")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadAvatar(IFormFile? avatarFile)
    {
        // 判断是否上传了头像
        if (avatarFile == null || avatarFile.Length == 0)
            return BadRequest(ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Any.FileNotUploaded
            ));

        // 判断头像文件大小
        if (avatarFile.Length > Constants.User.MaxAvatarSize)
            return BadRequest(ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Any.FileTooLarge
            ));

        // 判断头像文件类型
        var fileExtension = Path.GetExtension(avatarFile.FileName).ToLower();
        if (!Constants.User.AllowedAvatarExtensions.Contains(fileExtension))
            return BadRequest(ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Any.FileFormatNotSupported
            ));

        // 获取用户信息
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));

        var user = await repository.UserRepository.GetByIdAsync(userId.Value);
        if (user == null)
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.User.UserNotFound
            ));

        // 处理头像文件
        // 读取头像文件
        await using var avatarStream = avatarFile.OpenReadStream();
        Image image;
        try
        {
            image = await Image.LoadAsync(avatarStream);
        }
        catch
        {
            return BadRequest(ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Any.FileFormatNotSupported
            ));
        }

        // 判断头像文件尺寸
        if (image.Width > Constants.User.AvatarMaxWidth || image.Height > Constants.User.AvatarMaxHeight)
            return BadRequest(ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Any.ImageSizeTooLarge
            ));

        // 处理头像文件
        await using var processedImageStream = new MemoryStream();
        try
        {
            // 调整尺寸
            image.Mutate(ipc => ipc.Resize(new ResizeOptions
            {
                Size = new Size(Constants.User.AvatarStorageSize, Constants.User.AvatarStorageSize),
                Mode = ResizeMode.Max
            }));
            // 转换为 webp 格式
            await image.SaveAsWebpAsync(processedImageStream);
            processedImageStream.Seek(0, SeekOrigin.Begin);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to process avatar image");
            return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status500InternalServerError,
                ErrorMessages.Controller.Any.UnknownFileProcessingError)
            );
        }

        // 上传头像文件
        try
        {
            await repository.UserRepository.UpdateAvatarAsync(user, processedImageStream);
        }
        catch (Exception err)
        {
            logger.LogError(err, "Failed to update {} avatar", user.Id);
            return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status500InternalServerError,
                ErrorMessages.Controller.User.ProfileUpdateFailed
            ));
        }

        return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.User.AvatarUpdateSuccess));
    }

    /// <summary>
    ///     获取用户信息
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户信息</returns>
    [HttpGet("profile/{userId:Guid}", Name = "GetProfile")]
    [ProducesResponseType(typeof(UserProfileResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UserProfileResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(Guid userId)
    {
        var user = await repository.UserRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound(UserProfileResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.User.UserNotFound
            ));

        return Ok(UserProfileResp.Success(user));
    }

    /// <summary>
    ///     获取今日用户打卡状态
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>打卡状态</returns>
    [HttpGet("checkin/{userId:Guid}", Name = "GetCheckin")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UserProfileResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCheckin(Guid userId)
    {
        var user = await repository.UserRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound(UserProfileResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.User.UserNotFound
            ));

        if (await repository.UserCheckInRepository.CheckInTodayAsync(userId))
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.User.CheckInToday));

        return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.User.NoCheckInToday));
    }

    /// <summary>
    ///     获取用户的连续打卡天数
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>连续打卡天数</returns>
    [HttpGet("checkin/consecutive/{userId:Guid}", Name = "GetConsecutiveCheckInDays")]
    [ProducesResponseType(typeof(UserCheckInDayResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UserProfileResp), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetConsecutiveCheckInDays(Guid userId)
    {
        var user = await repository.UserRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound(UserProfileResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.User.UserNotFound
            ));

        var checkInRecords = await repository.UserCheckInRepository.GetUserCheckInsAsync(userId);
        var userCheckInDay = new UserCheckInDay
        {
            ContinuousDays = 0,
            TodayCheckInStatus = CheckIn.None
        };

        if (checkInRecords.Count == 0)
            return Ok(UserCheckInDayResp.Success(SuccessMessages.Controller.User.NoCheckInHistory, userCheckInDay));

        if (await repository.UserCheckInRepository.CheckInTodayAsync(userId))
            userCheckInDay.TodayCheckInStatus = CheckIn.Checked;

        var lastCheckInDate = checkInRecords.First().CheckInDate.Date;
        foreach (var record in checkInRecords.Skip(1))
        {
            var currentCheckInDate = record.CheckInDate.Date;
            if (lastCheckInDate == currentCheckInDate.AddDays(-1))
            {
                userCheckInDay.ContinuousDays++;
                lastCheckInDate = currentCheckInDate;
            }
            else
            {
                break;
            }
        }

        return Ok(UserCheckInDayResp.Success(SuccessMessages.Controller.User.GetConsecutiveCheckInDays,
            userCheckInDay));
    }
}