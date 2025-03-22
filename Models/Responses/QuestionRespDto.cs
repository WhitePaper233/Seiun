using Seiun.Resources;

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
        return new QuestionListResp(StatusCodes.Status200OK, SuccessMessages.Controller.Question.GetQuestionListSuccess,
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
        return new FillInBlankResp(StatusCodes.Status200OK, SuccessMessages.Controller.Question.GetFillInBlankSuccess,
            qes);
    }

    public static FillInBlankResp Fail(int code, string message)
    {
        return new FillInBlankResp(code, message, null);
    }
}

# endregion

# region Cloze Test

public class ClozeTestAnswerInfo
{
    public required int Key { get; set; }
    public required string Answer { get; set; }
    public required string Analysis { get; set; }
}

public class ClozeTestSelectionInfo
{
    public required int Key { get; set; }
    public required List<string> Words { get; set; }
}

public class ClozeTestInfo
{
    public required List<ClozeTestSelectionInfo> Selections { get; set; }
    public required string Content { get; set; }
    public required List<ClozeTestAnswerInfo> Answers { get; set; }
}

public sealed class ClozeTestResp(int code, string message, ClozeTestInfo? clozeTestInfoQuestion)
    : BaseRespWithData<ClozeTestInfo>(code, message, clozeTestInfoQuestion)
{
    public static ClozeTestResp Success(ClozeTestInfo qes)
    {
        return new ClozeTestResp(StatusCodes.Status200OK, SuccessMessages.Controller.Question.GetClozeTestSuccess,
            qes);
    }

    public static ClozeTestResp Fail(int code, string message)
    {
        return new ClozeTestResp(code, message, null);
    }
}

# endregion