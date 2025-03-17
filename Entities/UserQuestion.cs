using Seiun.Utils.Enums;

namespace Seiun.Entities;

public class UserQuestion : BaseEntity
{
    public required Guid UserId { get; set; }
    public required QuestionType Type { get; set; }
    public required Guid QuestionId { get; set; }
} 