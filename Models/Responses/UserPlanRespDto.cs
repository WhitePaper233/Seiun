using Seiun.Resources;

namespace Seiun.Models.Responses;

# region GetCurrentPlan

public class CurrentPlanData
{
    public required Guid WordBookId { get; set; }

    public required string WordBookName { get; set; }

    public required int SetDailyPlan { get; set; }

    public required int RemainingDays { get; set; }

    public required int LearnedCount { get; set; }

    public required long ExpectedCompletionAt { get; set; }
}

public sealed class CurrentWordBookResp(int code, string message, CurrentPlanData? currentPlanData)
    : BaseRespWithData<CurrentPlanData>(code, message, currentPlanData)
{
    public static CurrentWordBookResp Success(CurrentPlanData currentWordBook)
    {
        return new CurrentWordBookResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.UserPlan.GetCurrentPlanSuccess,
            currentWordBook);
    }

    public static CurrentWordBookResp Fail(int code, string message)
    {
        return new CurrentWordBookResp(code, message, null);
    }
}

# endregion