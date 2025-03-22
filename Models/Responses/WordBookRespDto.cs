using Seiun.Resources;

namespace Seiun.Models.Responses;

# region GetAllWordBooks

public class WordBook
{
    public required Guid WordBookId { get; set; }

    public required string WordBookName { get; set; }

    public required int WordCount { get; set; }

    public int LearnedWordCount { get; set; }

    public int DailyPlan { get; set; }

    public int RemainingDays { get; set; }
}

public class WordBookListData
{
    public required List<WordBook> WordBooks { get; set; }
}

public sealed class WordBookListResp(int code, string message, WordBookListData? wordBookListData)
    : BaseRespWithData<WordBookListData>(code, message, wordBookListData)
{
    public static WordBookListResp Success(List<WordBook> wordBooks)
    {
        return new WordBookListResp(StatusCodes.Status200OK, SuccessMessages.Controller.WordBook.GetWordBooksSuccess,
            new WordBookListData
            {
                WordBooks = wordBooks
            });
    }

    public static WordBookListResp Fail(int code, string message)
    {
        return new WordBookListResp(code, message, null);
    }
}

# endregion