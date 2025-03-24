using System.ComponentModel.DataAnnotations;
using Seiun.Resources;


namespace Seiun.Models.Parameters;

public class UpdatePlan
{
    [Required(ErrorMessage = ErrorMessages.ValidationError.DailyPlanRequired)]
    public required int DailyPlan { get; set; }
}