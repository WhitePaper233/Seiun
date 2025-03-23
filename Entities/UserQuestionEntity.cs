using System.ComponentModel.DataAnnotations.Schema;
using Seiun.Utils.Enums;

namespace Seiun.Entities;

public class UserQuestionEntity : BaseEntity
{
    public required Guid UserId { get; set; }
    public required ChallengeType Type { get; set; }
    public required Guid QuestionId { get; set; }

    [ForeignKey(nameof(this.UserId))] public virtual UserEntity User { get; set; } = null!;
}