using System.ComponentModel.DataAnnotations;
using Seiun.Resources;
using Seiun.Utils;
using Seiun.Utils.Enums;

namespace Seiun.Entities;

public class UserEntity : BaseEntity
{
    [EmailAddress(ErrorMessage = ErrorMessages.ValidationError.InvalidEmail)]
    [MaxLength(Constants.Email.MaxLength, ErrorMessage = ErrorMessages.ValidationError.OverEmailLength)]
    public string? Email { get; set; }

    [MaxLength(64)] public string? AvatarFileName { get; set; }

    [MaxLength(Constants.User.MaxDescriptionLength, ErrorMessage = ErrorMessages.ValidationError.OverDescriptionLength)]
    public string? Description { get; set; }

    [RegularExpression(RegExp.User.UserNamePattern, ErrorMessage = ErrorMessages.ValidationError.InvalidUserName)]
    [MaxLength(Constants.User.MaxUserNameLength, ErrorMessage = ErrorMessages.ValidationError.OverUserNameLength)]
    public required string UserName { get; set; }

    [MaxLength(Constants.User.MaxNickNameLength, ErrorMessage = ErrorMessages.ValidationError.OverNickNameLength)]
    public required string NickName { get; set; }

    [Phone(ErrorMessage = ErrorMessages.ValidationError.InvalidPhone)]
    [MaxLength(Constants.User.MaxPhoneNumberLength, ErrorMessage = ErrorMessages.ValidationError.OverPhoneNumberLength)]
    public required string PhoneNumber { get; set; }

    public required Gender Gender { get; set; }

    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }

    public required UserRole Role { get; set; }
    public required bool IsBanned { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual ICollection<UserTagEntity> UserTags { get; set; } = [];
    public virtual ICollection<UserCheckInEntity> CheckIns { get; set; } = [];
}