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
    public required List<OptionDetail> Options { get; set; }
    public required AnswerDetail Answer { get; set; }
    public required SelectedDetail SelectedWord { get; set; }
    public required string AnswerExampleSentence { get; set; }
    public required string SelectedWordExampleSentence { get; set; }
    public required MistakeStatus Status { get; set; }
}

public class SelectedDetail
{
    public required Guid WordId { get; set; }
    public required string Word { get; set; }
}

public sealed class MistakeDetailResp(int code, string message, MistakeDetail? mistake)
    : BaseRespWithData<MistakeDetail>(code, message, mistake)
{
    public static MistakeDetailResp Success(MistakeDetail mistake)
    {
        return new MistakeDetailResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.MistakeBook.GetMistakeDetailSuccess,
            mistake);
    }

    public static MistakeDetailResp Fail(int code, string message)
    {
        return new MistakeDetailResp(code, message, null);
    }
}

# endregion