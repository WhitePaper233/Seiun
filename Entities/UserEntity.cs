using System.ComponentModel.DataAnnotations;
using Seiun.Resources;
using Seiun.Utils;
using Seiun.Utils.Enums;
using System.Text.Json.Serialization;

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

    [JsonIgnore] public virtual ICollection<ArticleEntity> Articles { get; set; } = [];

    [JsonIgnore] public virtual ICollection<AiArticleEntity> AiArticles { get; set; } = [];

    [JsonIgnore] public virtual ICollection<ErrorWordRecordEntity> ErrorWordRecords { get; set; } = [];

    [JsonIgnore] public virtual ICollection<FinishedWordRecordEntity> FinishedWordRecords { get; set; } = [];

    [JsonIgnore] public virtual ICollection<PublicAnnouncementEntity> PublicAnnouncements { get; set; } = [];

    [JsonIgnore] public virtual ICollection<UserQuestionEntity> UserQuestions { get; set; } = [];

    [JsonIgnore] public virtual ICollection<UserPlanEntity> UserPlans { get; set; } = null!;

    [JsonIgnore] public virtual ICollection<WordSessionEntity> WordSession { get; set; } = null!;

    [JsonIgnore] public virtual ICollection<UserCheckInEntity> CheckIns { get; set; } = [];
}