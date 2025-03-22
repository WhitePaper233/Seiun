using Seiun.Resources;

namespace Seiun.Models.Responses;

# region GetAllWordBank

public class WordBook
{
    public required Guid WordBookId { get; set; }

    public required string WordBookName { get; set; }

    public required int WordCount { get; set; }

    public int LearnedWordCount { get; set; }

    public int DailyPlan { get; set; }

    public int RemainingDays { get; set; }
}

public class AllWordBookDetail
{
    public required List<WordBook> WordBooks { get; set; }
}

public sealed class WordBooksResp(int code, string message, AllWordBookDetail? wordBanks)
    : BaseRespWithData<AllWordBookDetail>(code, message, wordBanks)
{
    public static WordBooksResp Success(List<WordBook> wordBanks)
    {
        return new WordBooksResp(StatusCodes.Status200OK, SuccessMessages.Controller.UserPlan.GetWordBooksSuccess,
            new AllWordBookDetail
            {
                WordBooks = wordBanks
            });
    }

    public static WordBooksResp Fail(int code, string message)
    {
        return new WordBooksResp(code, message, null);
    }
}

# endregion

# region GetCurrentWordBank

public class CurrentWordBankDetail
{
    public required Guid WordBookId { get; set; }

    public required string WordBookName { get; set; }

    public required int SetDailyPlan { get; set; }

    public required int RemainingDays { get; set; }

    public required int LearnedCount { get; set; }

    public required DateTimeOffset ExpectedCompletionAt { get; set; }
}

public sealed class CurrentWordBankResp(int code, string message, CurrentWordBankDetail? wordBanks)
    : BaseRespWithData<CurrentWordBankDetail>(code, message, wordBanks)
{
    public static CurrentWordBankResp Success(CurrentWordBankDetail currentWordBook)
    {
        return new CurrentWordBankResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.UserPlan.GetCurrentWordBookSuccess,
            currentWordBook);
    }

    public static CurrentWordBankResp Fail(int code, string message)
    {
        return new CurrentWordBankResp(code, message, null);
    }
}

# endregion