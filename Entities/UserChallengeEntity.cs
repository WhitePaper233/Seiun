using System.ComponentModel.DataAnnotations.Schema;
using Seiun.Utils.Enums;

namespace Seiun.Entities;

public class UserChallengeEntity : BaseEntity
{
    public required Guid UserId { get; set; }
    public required ChallengeType Type { get; set; }
    public required Guid ChallengeId { get; set; }

    [ForeignKey(nameof(this.UserId))] public virtual UserEntity User { get; set; } = null!;
}