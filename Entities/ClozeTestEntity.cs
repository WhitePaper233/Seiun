

namespace Seiun.Entities;

public class ClozeTestAnswerEntity : BaseEntity
{
    public required Guid QuestionId { get; set; }
    public required int Key { get; set; }
    public required string Answer { get; set; }
    public required string Analysis { get; set; }
}

public class ClozeTestSelectionEntity : BaseEntity
{
    public required Guid QuestionId { get; set; }
    public required int Key { get; set; }
    public required List<string> Words { get; set; }
}

public class ClozeTestEntity : BaseEntity
{
    public required string Content { get; set; }
}