
namespace Seiun.Entities;

public class FillInBlankAnswerEntity : BaseEntity
{
    public required Guid QuestionId { get; set; }
    public required int Key { get; set; }
    public required string Answer { get; set; }
    public required string Analysis { get; set; }
}

public class FillInBlankWordEntity : BaseEntity
{
    public required Guid QuestionId { get; set; }
    public required string Word { get; set; }
}

public class FillInBlankEntity : BaseEntity
{ 
    public required string Content { get; set; }
    public required string Transition { get; set; }
}

