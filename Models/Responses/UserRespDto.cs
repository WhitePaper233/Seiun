using Seiun.Entities;
using Seiun.Resources;
using Seiun.Utils.Enums;

namespace Seiun.Models.Responses;

#region UserLoginResponse

/// <summary>
/// Token 信息
/// </summary>
public class TokenInfo
{
    public required string Token { get; set; }
    public required long ExpireAt { get; set; }
}

/// <summary>
/// 用户登录响应
/// </summary>
public sealed class UserLoginResp(int code, string message, TokenInfo? tokenInfo)
    : BaseRespWithData<TokenInfo>(code, message, tokenInfo)
{
    public static UserLoginResp Success(string message, TokenInfo tokenInfo)
    {
        return new UserLoginResp(200, message, tokenInfo);
    }

    public static UserLoginResp Fail(int code, string message)
    {
        return new UserLoginResp(code, message, null);
    }
}

#endregion

#region UserProfileResponse

/// <summary>
/// 用户资料
/// </summary>
public class UserProfile
{
    public required string UserName { get; set; }
    public required string NickName { get; set; }
    public required string AvatarUrl { get; set; }
    public required Gender Gender { get; set; }
}

/// <summary>
/// 用户资料响应
/// </summary>
public sealed class UserProfileResp(int code, string message, UserProfile? userProfile)
    : BaseRespWithData<UserProfile>(code, message, userProfile)
{
    public static UserProfileResp Success(UserEntity userEntity)
    {
        return new UserProfileResp(StatusCodes.Status200OK, SuccessMessages.Controller.User.GetProfileSuccess,
            new UserProfile
            {
                UserName = userEntity.UserName,
                NickName = userEntity.NickName,
                AvatarUrl = $"/resources/avatar/{userEntity.AvatarFileName}",
                Gender = userEntity.Gender
            });
    }

    public static UserLoginResp Fail(int code, string message)
    {
        return new UserLoginResp(code, message, null);
    }
}
#endregion

#region UserChickInDayResponse

public class UserCheckInDay
{
    public required int ContinuousDays { get; set; }
    public required CheckIn TodayCheckInStatus { get; set; }
}

public sealed class UserCheckInDayResp(int code, string message, UserCheckInDay? userChickInDay)
    : BaseRespWithData<UserCheckInDay>(code, message, userChickInDay)
{
    public static UserCheckInDayResp Success(string message, UserCheckInDay userChickInDay)
    {
        return new UserCheckInDayResp(200, message, userChickInDay);
    }

    public static UserCheckInDayResp Fail(int code, string message)
    {
        return new UserCheckInDayResp(code, message, null);
    }
}
#endregion

#region UserListResponse

public class UserList
{
    public required Guid UserId { get; set; }
    public required UserRole Role { get; set; }
    public required string UserName { get; set; }
    public string? Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required Gender Gender { get; set; }
    public required string NickName { get; set; }
    public required string? Description { get; set; }
    public required DateTime LastCheckInTime { get; set; }
}

public class UserListData
{
    public required List<UserList> List { get; set; }
    public required int Total { get; set; }
}

public sealed class UserListResp(int code, string message, UserListData? userList)
    : BaseRespWithData<UserListData>(code, message, userList)
{
    public static UserListResp Success(string message, UserListData userList) 
    {
        return new UserListResp(200, message, userList);
    }

    public static UserListResp Fail(int code, string message)
    {
        return new UserListResp(code, message, null);
    }
}

#endregion