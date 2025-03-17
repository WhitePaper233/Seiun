
using Seiun.Resources;

namespace  Seiun.Models.Responses;

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
        return new FillInBlankResp(StatusCodes.Status200OK, SuccessMessages.Controller.Question.GetFillInBlankSuccess,
            qes);
    }

    public static FillInBlankResp Fail(int code, string message)
    {
        return new FillInBlankResp(code, message, null);
    }
}

# endregion

