using Seiun.Entities;
using Seiun.Resources;

namespace Seiun.Models.Responses;

# region StartStudyResponse

public class WordSessionDetail
{
	public required Guid WordSessionId { get; set; }
	public required int ReviewingWordCount { get; set; }
	public required int StudyingWordCount { get; set; }
}

public sealed class StartStudyResp(int code, string message, WordSessionDetail? WordsessionDetail)
	: BaseRespWithData<WordSessionDetail>(code, message, WordsessionDetail)
{
	public static StartStudyResp Success(Guid sessionId, int reviewingWordCount, int studyingWordCount)
	{
		return new StartStudyResp(StatusCodes.Status200OK, SuccessMessages.Controller.StudySession.GetSessionDetailSuccess,
			new WordSessionDetail
			{
				WordSessionId = sessionId,
				ReviewingWordCount = reviewingWordCount,
				StudyingWordCount = studyingWordCount
			});
	}

	public static StartStudyResp Fail(int code, string message)
	{
		return new StartStudyResp(code, message, null);
	}
}

# endregion

# region GetNextWordResponse

public class NextWordDetail
{
	public required List<OptionDetail> Options { get; set; }
	public required AnswerDetail Answer { get; set; }
}

public class OptionDetail
{
	public required Guid WordId { get; set; }
	public required string Word { get; set; }
	public string? Pronunciation { get; set; }
	public required string Definition { get; set; }
}

public class AnswerDetail
{
	public required Guid WordId { get; set; }
	public required string Word { get; set; }
}


public sealed class GetNextWordResp(int code, string message, NextWordDetail? nextWordDetail)
	: BaseRespWithData<NextWordDetail>(code, message, nextWordDetail)
{
	public static GetNextWordResp Success(WordEntity nextWord)
	{
		var distractedWords = nextWord.Distractors.Select(d => d.DistractedWord).ToList();
		var options = distractedWords.Select(d => 
			new OptionDetail { WordId = d.Id, Word = d.WordText, Pronunciation = d.Pronunciation, Definition = d.Definition })
		.ToList();
		var answer = new AnswerDetail { WordId = nextWord.Id, Word = nextWord.WordText };
		
		return new GetNextWordResp(StatusCodes.Status200OK, SuccessMessages.Controller.StudySession.GetNextWordSuccess,
			new NextWordDetail
			{
				Options = options,
				Answer = answer
			});
	}

	public static GetNextWordResp Fail(int code, string message)
	{
		return new GetNextWordResp(code, message, null);
	}
}

# endregion