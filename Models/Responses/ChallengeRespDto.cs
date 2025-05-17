using Seiun.Resources;
using Seiun.Utils.Enums;
using System.Text.Json.Serialization;

namespace Seiun.Models.Responses;

# region Question List

public class ChallengeListInfo
{
	public required List<Guid>? QuestionIds { get; set; }
}

public sealed class ChallengeListResp(int code, string message, ChallengeListInfo? question)
	: BaseRespWithData<ChallengeListInfo>(code, message, question)
{
	public static ChallengeListResp Success(List<Guid>? qesIds) => new(StatusCodes.Status200OK,
	SuccessMessages.Controller.Challenge.GetChallengeListSuccess,
	new ChallengeListInfo
	{
		QuestionIds = qesIds
	});

	public static ChallengeListResp Fail(int code, string message) => new(code, message, null);
}

# endregion

# region Fill in Blank

public class FillInBlankDetail
{
	[JsonPropertyName("type")] public required ChallengeType Type { get; set; }
	[JsonPropertyName("content")] public required string Content { get; set; }
	[JsonPropertyName("selections")] public required List<string> Selections { get; set; }
	[JsonPropertyName("answers")] public required Dictionary<string, string> Answers { get; set; }
}

public sealed class FillInBlankResp(int code, string message, FillInBlankDetail? fillInBlankQuestion)
	: BaseRespWithData<FillInBlankDetail>(code, message, fillInBlankQuestion)
{
	public static FillInBlankResp Success(FillInBlankDetail fillInBlank) => new(StatusCodes.Status200OK,
	SuccessMessages.Controller.Challenge.GetFillInBlankSuccess,
	fillInBlank);

	public static FillInBlankResp Fail(int code, string message) => new(code, message, null);
}

# endregion

# region Cloze Test

public class ClozeTestDetail
{
	[JsonPropertyName("type")] public required ChallengeType Type { get; set; }
	[JsonPropertyName("content")] public required string Content { get; set; }
	[JsonPropertyName("selections")] public required Dictionary<string, List<string>> Selections { get; set; }
	[JsonPropertyName("answers")] public required Dictionary<string, string> Answers { get; set; }
	[JsonPropertyName("analysis")] public required Dictionary<string, string> Analysis { get; set; }
}

public sealed class ClozeTestResp(int code, string message, ClozeTestDetail? clozeTest)
	: BaseRespWithData<ClozeTestDetail>(code, message, clozeTest)
{
	public static ClozeTestResp Success(ClozeTestDetail clozeTest) => new(StatusCodes.Status200OK,
	SuccessMessages.Controller.Challenge.GetClozeTestSuccess,
	clozeTest);

	public static ClozeTestResp Fail(int code, string message) => new(code, message, null);
}

# endregion
