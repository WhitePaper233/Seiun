using System.ComponentModel.DataAnnotations;
using Seiun.Resources;
using Seiun.Utils;
using Seiun.Utils.Attributes;
using Seiun.Utils.Enums;

namespace Seiun.Models.Parameters;

public class UserRegister
{
    // 手机号
    [Required(ErrorMessage = ErrorMessages.ValidationError.PhoneRequired)]
    [Phone(ErrorMessage = ErrorMessages.ValidationError.InvalidPhone)]
    public required string PhoneNumber { get; set; }

    // 密码
    [Required(ErrorMessage = ErrorMessages.ValidationError.PasswordRequired)]
    [RegularExpression(RegExp.User.PasswordPattern, ErrorMessage = ErrorMessages.ValidationError.InvalidPassword)]
    public required string Password { get; set; }
}

[AtLeastOnePropertyRequired([nameof(PhoneNumber), nameof(Email), nameof(UserName)],
    ErrorMessage = ErrorMessages.ValidationError.AtLeastOnePropertyRequired)]
public class UserLogin
{
    // 手机号
    [Phone(ErrorMessage = ErrorMessages.ValidationError.InvalidPhone)]
    public string? PhoneNumber { get; set; }

    // 邮箱
    [EmailAddress(ErrorMessage = ErrorMessages.ValidationError.InvalidEmail)]
    public string? Email { get; set; }

    // 用户名
    [RegularExpression(RegExp.User.UserNamePattern, ErrorMessage = ErrorMessages.ValidationError.InvalidUserName)]
    public string? UserName { get; set; }

    // 密码
    [Required(ErrorMessage = ErrorMessages.ValidationError.PasswordRequired)]
    [RegularExpression(RegExp.User.PasswordPattern, ErrorMessage = ErrorMessages.ValidationError.InvalidPassword)]
    public required string Password { get; set; }
}

[AtLeastOnePropertyRequired(ErrorMessage = ErrorMessages.ValidationError.AtLeastOnePropertyRequired)]
public class UserUpdateProfile
{
    // 昵称
    [MaxLength(Constants.User.MaxNickNameLength, ErrorMessage = ErrorMessages.ValidationError.OverNickNameLength)]
    public string? NickName { get; set; }

    // 性别
    public Gender? Gender { get; set; }

    // 描述
    [MaxLength(Constants.User.MaxDescriptionLength, ErrorMessage = ErrorMessages.ValidationError.OverDescriptionLength)]
    public string? Description { get; set; }
}

public class UserUpdateByAdmin
{
    [Required(ErrorMessage = ErrorMessages.ValidationError.UserIdRequired)]
    public required Guid UserId { get; set; }

    [MaxLength(Constants.User.MaxNickNameLength, ErrorMessage = ErrorMessages.ValidationError.OverNickNameLength)]
    public required string NickName { get; set; }

    public Gender? Gender { get; set; }


    [MaxLength(Constants.User.MaxDescriptionLength, ErrorMessage = ErrorMessages.ValidationError.OverDescriptionLength)]
    public string? Description { get; set; }
}

public class GetUsersByAdmin
{
    public required int Index { get; set; }
    public required int Size { get; set; }
    public string? Keyword { get; set; }
}