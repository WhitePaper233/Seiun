using Seiun.Resources;
using Seiun.Utils;
using System.ComponentModel.DataAnnotations;

namespace Seiun.Entities;

public class FillInBlankAnswerEntity : BaseEntity
{
    public required Guid QuestionId { get; set; }
    public required int Key { get; set; }
    
    [MaxLength(Constants.Question.MaxQuestionAnswerLength, ErrorMessage = ErrorMessages.ValidationError.OverMaxQuestionAnswerLength)]
    public required string Answer { get; set; }
    [MaxLength(Constants.Question.MaxQuestionAnalysisLength, ErrorMessage = ErrorMessages.ValidationError.OverMaxQuestionAnalysisLength)]
    public required string Analysis { get; set; }
}

public class FillInBlankWordEntity : BaseEntity
{
    public required Guid QuestionId { get; set; }
    
    [MaxLength(Constants.Question.MaxQuestionWordLength, ErrorMessage = ErrorMessages.ValidationError.OverMaxQuestionWordLength)]
    public required string Word { get; set; }
}

public class FillInBlankEntity : BaseEntity
{
    [MaxLength(Constants.Question.MaxQuestionContentLength, ErrorMessage = ErrorMessages.ValidationError.OverMaxQuestionContentLength)]
    public required string Content { get; set; }
    [MaxLength(Constants.Question.MaxQuestionTransitionLength, ErrorMessage = ErrorMessages.ValidationError.OverMaxQuestionTransitionLength)]
    public required string Transition { get; set; }
}

