using System.ComponentModel.DataAnnotations.Schema;

namespace Seiun.Entities;

public class UserPlanEntity : BaseEntity
{
    public required Guid UserId { get; set; }

    public required Guid WordBookId { get; set; }

    public required int DailyPlan { get; set; }

    public required int LearnedCount { get; set; }

    [ForeignKey(nameof(this.UserId))] public virtual UserEntity User { get; set; } = null!;
}