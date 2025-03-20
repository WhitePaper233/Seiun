using System.ComponentModel.DataAnnotations.Schema;
using Seiun.Utils.Enums;

namespace Seiun.Entities;
public class UserTagEntity : BaseEntity
{
    public required Guid UserId { get; set; }

    public required WordLevel WordLevel { get; set; }

    public required int SetDailyPlan { get; set; }

    public required int SetTotalDays { get; set; }
    
    // 剩余单词 / 每日计划
    public required int RemainingDays { get; set; }

    public required int LearnedCount { get; set; }
    
    public required DateTimeOffset ExpectedCompletionAt { get; set; }
    
    // DateTimeOffset 与 DateTime一样可以用过Api直接对年月日运算，格式化后也能正确显示
    public required DateTimeOffset? LastStudyAt { get; set; }
    
    public required DateTimeOffset? SettingAt { get; set; }

    [ForeignKey(nameof(this.UserId))] 
    public virtual UserEntity User { get; set; } = null!;
}
