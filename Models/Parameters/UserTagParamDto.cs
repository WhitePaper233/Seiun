using System.ComponentModel.DataAnnotations;
using Seiun.Resources;
using Seiun.Utils.Enums;


namespace Seiun.Models.Parameters;

public class UserSelectWordBank
{
    [Required(ErrorMessage = ErrorMessages.ValidationError.UserTagWordLevelRequired)]
    public required WordLevel WordLevel { get; set; }

    [Required(ErrorMessage = ErrorMessages.ValidationError.UserTagSetDailyPlanRequired)]
    public required int SetDailyPlan { get; set; }

    [Required(ErrorMessage = ErrorMessages.ValidationError.UserTagSetTotalDaysRequired)]
    public required int SetTotalDays { get; set; }
    
    [Required(ErrorMessage = ErrorMessages.ValidationError.UserTagExpectedCompletionAtRequired)]
    public required DateTimeOffset ExpectedCompletionAt { get; set; }
}

public class UserUpdatePlan
{
    [Required(ErrorMessage = ErrorMessages.ValidationError.UserTagWordLevelRequired)]
    public required int SetDailyPlan { get; set; }
    [Required(ErrorMessage = ErrorMessages.ValidationError.UserTagSetTotalDaysRequired)]
    public required int SetTotalDays { get; set; }
}