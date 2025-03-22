using System.ComponentModel.DataAnnotations;
using Seiun.Utils;
using Seiun.Resources;

namespace Seiun.Entities;

public class ClozeTestAnswerEntity : BaseEntity
{
    public required Guid QuestionId { get; set; }
    public required int Key { get; set; }
    
    [MaxLength(Constants.Question.MaxQuestionAnswerLength, ErrorMessage = ErrorMessages.ValidationError.OverMaxQuestionAnswerLength)]
    public required string Answer { get; set; }
    
    [MaxLength(Constants.Question.MaxQuestionAnalysisLength, ErrorMessage = ErrorMessages.ValidationError.OverMaxQuestionAnalysisLength)]
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
    [MaxLength(Constants.Question.MaxQuestionContentLength, ErrorMessage = ErrorMessages.ValidationError.OverMaxQuestionContentLength)]
    public required string Content { get; set; }
}