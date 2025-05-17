using System.ComponentModel.DataAnnotations;
using Seiun.Resources;

namespace Seiun.Models.Parameters;

public class SelectWordBook
{
	[Required(ErrorMessage = ErrorMessages.ValidationError.WordBookIdRequired)]
	public required Guid WordBookId { get; set; }

	[Required(ErrorMessage = ErrorMessages.ValidationError.DailyPlanRequired)]
	public required int DailyPlan { get; set; }
}
