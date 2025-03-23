using Seiun.Resources;
using Seiun.Utils.Enums;
using System.Text.Json.Serialization;

namespace Seiun.Models.Responses;

# region Question List

public class QuestionListInfo
{
    public required List<Guid> QuestionIds { get; set; }
}

public sealed class QuestionListResp(int code, string message, QuestionListInfo? question)
    : BaseRespWithData<QuestionListInfo>(code, message, question)
{
    public static QuestionListResp Success(List<Guid> qesIds)
    {
        return new QuestionListResp(StatusCodes.Status200OK, SuccessMessages.Controller.Challenge.GetQuestionListSuccess,
            new QuestionListInfo
            {
                QuestionIds = qesIds
            });
    }

    public static QuestionListResp Fail(int code, string message)
    {
        return new QuestionListResp(code, message, null);
    }
}

# endregion

# region Fill in Blank

public class FillInBlankAnswerInfo
{
    public required int Key { get; set; }
    public required string Answer { get; set; }
    public required string Analysis { get; set; }
}

public class FillInBlankInfo
{
    public required List<string> Words { get; set; }
    public required string Content { get; set; }
    public required string Transition { get; set; }
    public required List<FillInBlankAnswerInfo> Answers { get; set; }
}

public sealed class FillInBlankResp(int code, string message, FillInBlankInfo? fillInBlankQuestion)
    : BaseRespWithData<FillInBlankInfo>(code, message, fillInBlankQuestion)
{
    public static FillInBlankResp Success(FillInBlankInfo qes)
    {
        return new FillInBlankResp(StatusCodes.Status200OK, SuccessMessages.Controller.Challenge.GetFillInBlankSuccess,
            qes);
    }

    public static FillInBlankResp Fail(int code, string message)
    {
        return new FillInBlankResp(code, message, null);
    }
}

# endregion

# region Cloze Test

public class ClozeTestDetail
{
    [JsonPropertyName("type")]
    public required ChallengeType Type { get; set; }
    [JsonPropertyName("content")]
    public required string Content { get; set; }
    [JsonPropertyName("selections")]
    public required Dictionary<string, List<string>> Selections { get; set; }
    [JsonPropertyName("answers")]
    public required Dictionary<string, string> Answers { get; set; }
}

public sealed class ClozeTestResp(int code, string message, ClozeTestDetail? clozeTest)
    : BaseRespWithData<ClozeTestDetail>(code, message, clozeTest)
{
    public static ClozeTestResp Success(ClozeTestDetail clozeTest)
    {
        return new ClozeTestResp(StatusCodes.Status200OK, SuccessMessages.Controller.Challenge.GetClozeTestSuccess,
            clozeTest);
    }

    public static ClozeTestResp Fail(int code, string message)
    {
        return new ClozeTestResp(code, message, null);
    }
}

# endregion