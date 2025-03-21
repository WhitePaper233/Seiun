using System.ComponentModel.DataAnnotations;
using Seiun.Resources;
using Seiun.Utils.Enums;


namespace Seiun.Models.Parameters;

public class UserSelectWordBank
{
    [Required(ErrorMessage = ErrorMessages.ValidationError.WordBookIdRequired)]
    public required Guid WordBookId { get; set; }

    [Required(ErrorMessage = ErrorMessages.ValidationError.DailyPlanRequired)]
    public required int SetDailyPlan { get; set; }
}

public class UserUpdatePlan
{
    [Required(ErrorMessage = ErrorMessages.ValidationError.DailyPlanRequired)]
    public required int SetDailyPlan { get; set; }
}