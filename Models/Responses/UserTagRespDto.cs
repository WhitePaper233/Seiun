using Seiun.Entities;
using Seiun.Utils.Enums;
using Seiun.Resources;
using Seiun.Utils;

namespace Seiun.Models.Responses;

# region GetAllWordBank 

public class WordBank
{
    public required WordLevel WordLevel { get; set; }
    
    public required int WordCount { get; set; }

    public int LearnedWordCount { get; set; } = 0;

    public int DailyPlan { get; set; } = 0;

    public int RemainingDays { get; set; } = 0;

    public DateTimeOffset? LastStudyAt { get; set; } = null;
}

public class AllWordBankDetail
{
    public required List<WordBank> WordBanks { get; set; }
}

public sealed class WordBanksResp(int code, string message, AllWordBankDetail? wordBanks)
    : BaseRespWithData<AllWordBankDetail>(code, message, wordBanks)
{
    public static WordBanksResp Success(List<WordBank> wordBanks)
    {
        return new WordBanksResp(StatusCodes.Status200OK, SuccessMessages.Controller.UserTag.GetWordBanksSuccess,
            new AllWordBankDetail
            {
                WordBanks = wordBanks
            });
    }

    public static WordBanksResp Fail(int code, string message)
    {
        return new WordBanksResp(code, message, null);
    }
}

# endregion

# region GetCurrentWordBank

public class CurrentWordBankDetail
{
    public required WordLevel WordLevel { get; set; }

    public required int SetDailyPlan { get; set; }

    public required int SetTotalDays { get; set; }
    
    public required int RemainingDays { get; set; }

    public required int LearnedCount { get; set; }
    
    public required DateTimeOffset ExpectedCompletionAt { get; set; }
    
    public required DateTimeOffset? LastStudyAt { get; set; }
}

public sealed class CurrentWordBankResp(int code, string message, CurrentWordBankDetail? wordBanks)
    : BaseRespWithData<CurrentWordBankDetail>(code, message, wordBanks)
{
    public static CurrentWordBankResp Success(UserTagEntity userTag)
    {
        return new CurrentWordBankResp(StatusCodes.Status200OK, SuccessMessages.Controller.UserTag.GetCurrentWordBankSuccess,
            new CurrentWordBankDetail
            {
                WordLevel = userTag.WordLevel,
                SetDailyPlan = userTag.SetDailyPlan,
                SetTotalDays = userTag.SetTotalDays,
                RemainingDays = userTag.RemainingDays,
                LearnedCount = userTag.LearnedCount,
                ExpectedCompletionAt = userTag.ExpectedCompletionAt,
                LastStudyAt = userTag.LastStudyAt,
            });
    }

    public static CurrentWordBankResp Fail(int code, string message)
    {
        return new CurrentWordBankResp(code, message, null);
    }
}

# endregion