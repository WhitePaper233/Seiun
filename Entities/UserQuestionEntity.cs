using System.ComponentModel.DataAnnotations.Schema;
using Seiun.Utils.Enums;

namespace Seiun.Entities;

public class UserQuestionEntity : BaseEntity
{
    public required Guid UserId { get; set; }
    public required QuestionType Type { get; set; }
    public required Guid QuestionId { get; set; }
    public required DateTimeOffset SettingAt { get; set; }
    
    [ForeignKey(nameof(this.UserId))]
    public virtual UserEntity User { get; set; } = null!;
} 