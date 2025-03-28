using Seiun.Entities;
using Seiun.Resources;
using Seiun.Utils.Enums;

namespace Seiun.Models.Responses;

# region Mistake List

public class MistakeListDetail
{
    public required List<Guid>? MistakeList { get; set; }
}

public sealed class MistakeListResp(int code, string message, MistakeListDetail? mistakeList)
    : BaseRespWithData<MistakeListDetail>(code, message, mistakeList)
{
    public static MistakeListResp Success(List<Guid>? mistakeList)
    {
        return new MistakeListResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.MistakeBook.GetMistakeListSuccess,
            new MistakeListDetail
            {
                MistakeList = mistakeList
            });
    }

    public static MistakeListResp Fail(int code, string message)
    {
        return new MistakeListResp(code, message, null);
    }
}

# endregion

# region MistakeDetail

public class MistakeDetail
{
    public required string WordText { get; set; }
    public string? Pronunciation { get; set; }
    public required string Definition { get; set; }
    public required string ExampleSentence { get; set; }
}

public sealed class MistakeDetailResp(int code, string message, MistakeDetail? mistakeWord)
    : BaseRespWithData<MistakeDetail>(code, message, mistakeWord)
{
    public static MistakeDetailResp Success(WordEntity mistakeWord)
    {
        return new MistakeDetailResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.MistakeBook.GetMistakeWordSuccess,
            new MistakeDetail
            {
                WordText = mistakeWord.WordText,
                Pronunciation = mistakeWord.Pronunciation,
                Definition = mistakeWord.Definition,
                ExampleSentence = mistakeWord.ExampleSentence
            });
    }

    public static MistakeDetailResp Fail(int code, string message)
    {
        return new MistakeDetailResp(code, message, null);
    }
}

# endregion

# region Mistakes

public sealed class MistakesResp(int code, string message, List<MistakeDetail>? mistakeWordDetails)
    : BaseRespWithData<List<MistakeDetail>>(code, message, mistakeWordDetails)
{
    public static MistakesResp Success(List<WordEntity> mistakeWordDetails)
    {
        return new MistakesResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.MistakeBook.GetMistakeWordSuccess,
            mistakeWordDetails.Select(a =>
                new MistakeDetail
                {
                    WordText = a.WordText,
                    Pronunciation = a.Pronunciation,
                    Definition = a.Definition,
                    ExampleSentence = a.ExampleSentence
                }).ToList());
    }

    public static MistakesResp Fail(int code, string message)
    {
        return new MistakesResp(code, message, null);
    }
}

# endregion