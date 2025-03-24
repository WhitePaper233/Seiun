using System.ComponentModel.DataAnnotations.Schema;
using Seiun.Utils.Enums;


namespace Seiun.Entities;

public class ChallengeEntity : BaseEntity
{
    public required Guid UserId { get; set; }
    public required Guid SessionId { get; set; }
    public required ChallengeType Type { get; set; }
    [Column(TypeName = "TEXT")] public required string ChallengeJson { get; set; }
    [ForeignKey(nameof(this.SessionId))] public virtual WordSessionEntity Session { get; set; } = null!;
}