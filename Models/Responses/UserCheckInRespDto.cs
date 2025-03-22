using Seiun.Resources;

namespace Seiun.Models.Responses;

# region UserCheckInResp

public class UserCheckInDetail
{
    public required bool TodayUserIsCheckIn { get; set; }
    public required int DailyPlan { get; set; }
    public required int TodayPlanReviewedCount { get; set; }
    public required int ToDayStudiedCount { get; set; }
    public required int ToDayReviewedCount { get; set; }
}

public sealed class UserCheckInResp(int code, string message, UserCheckInDetail? checkInDetail)
    : BaseRespWithData<UserCheckInDetail>(code, message, checkInDetail)
{
    public static UserCheckInResp Success(UserCheckInDetail checkInDetail)
    {
        return new UserCheckInResp(StatusCodes.Status200OK, SuccessMessages.Controller.CheckIn.GetCheckInStatusSuccess,
            checkInDetail);
    }

    public static UserCheckInResp Fail(int code, string message)
    {
        return new UserCheckInResp(code, message, null);
    }
}

# endregion

# region ConsecutiveCheckInDaysResp

public class ConsecutiveCheckInDaysDetail
{
    public required int Days { get; set; }
}

public sealed class ConsecutiveCheckInDaysResp(int code, string message, ConsecutiveCheckInDaysDetail? days)
    : BaseRespWithData<ConsecutiveCheckInDaysDetail>(code, message, days)
{
    public static ConsecutiveCheckInDaysResp Success(ConsecutiveCheckInDaysDetail days)
    {
        return new ConsecutiveCheckInDaysResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.CheckIn.GetCheckInDaysSuccess,
            days);
    }
}

# endregion