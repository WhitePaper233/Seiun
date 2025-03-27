using Seiun.Entities;
using Seiun.Resources;

namespace Seiun.Models.Responses;

# region StartStudyResponse

public class WordDetail
{
    public required string WordText { get; set; }
    public string? Pronunciation { get; set; }
    public required string Definition { get; set; }
}

public class WordSessionDetail
{
    public required Guid SessionId { get; set; }
    public required int ReviewingWordCount { get; set; }
    public required int StudyingWordCount { get; set; }
    public required List<WordDetail> Words { get; set; }
}

public sealed class StartStudyResp(int code, string message, WordSessionDetail? wordSessionDetail)
    : BaseRespWithData<WordSessionDetail>(code, message, wordSessionDetail)
{
    public static StartStudyResp Success(Guid sessionId, int reviewingWordCount, int studyingWordCount,
        Queue<WordEntity> wordQueue)
    {
        return new StartStudyResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.StudySession.GetSessionDetailSuccess,
            new WordSessionDetail
            {
                SessionId = sessionId,
                ReviewingWordCount = reviewingWordCount,
                StudyingWordCount = studyingWordCount,
                Words = wordQueue.Select(a =>
                    new WordDetail
                    {
                        WordText = a.WordText,
                        Pronunciation = a.Pronunciation,
                        Definition = a.Definition
                    }).ToList()
            });
    }

    public static StartStudyResp Fail(int code, string message)
    {
        return new StartStudyResp(code, message, null);
    }
}

# endregion

# region ContinueStudy

public class ContinueStudyDetail
{
    public required Guid SessionId { get; set; }
    public required int ReviewingWordCount { get; set; }
    public required int StudyingWordCount { get; set; }
    public required List<WordDetail> Words { get; set; }
}

public sealed class ContinueStudyResp(int code, string message, ContinueStudyDetail? sessionDetail)
    : BaseRespWithData<ContinueStudyDetail>(code, message, sessionDetail)
{
    public static ContinueStudyResp Success(Guid sessionId, Queue<WordEntity> studyingWordQueue,
        WordSessionEntity wordSession)
    {
        return new ContinueStudyResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.StudySession.ContinueSessionSuccess,
            new ContinueStudyDetail
            {
                SessionId = sessionId,
                ReviewingWordCount = wordSession.ReviewingWords?.Count ?? 0,
                StudyingWordCount = wordSession.StudyingWords?.Count ?? 0,
                Words = studyingWordQueue.Select(w =>
                    new WordDetail
                    {
                        WordText = w.WordText,
                        Pronunciation = w.Pronunciation,
                        Definition = w.Definition
                    }).ToList()
            });
    }
}

# endregion

# region GetNextWordResponse

public class NextWordDetail
{
    public required List<OptionDetail> Options { get; set; }
    public required AnswerDetail Answer { get; set; }
    public required int ReviewingWordCount { get; set; }
    public required int StudyingWordCount { get; set; }
    public required string ExampleSentence { get; set; }
}

public class OptionDetail
{
    public required Guid WordId { get; set; }
    public required string Word { get; set; }
    public required string Pronunciation { get; set; }
    public required string Definition { get; set; }
    public required string PrimaryDefinition  { get; set; }
}

public class AnswerDetail
{
    public required Guid WordId { get; set; }
    public required string Word { get; set; }
}

public sealed class GetNextWordResp(int code, string message, NextWordDetail? nextWordDetail)
    : BaseRespWithData<NextWordDetail>(code, message, nextWordDetail)
{
    public static GetNextWordResp Success(WordEntity nextWord, List<WordEntity> distractorWords, int reviewingWordCount,
        int studyingWordCount)
    {
        var options = distractorWords.Select(d =>
                new OptionDetail
                    { WordId = d.Id, Word = d.WordText, Pronunciation = d.Pronunciation, Definition = d.Definition, PrimaryDefinition = d.PrimaryDefinition})
            .ToList();
        options.Add(new OptionDetail
        {
            WordId = nextWord.Id, Word = nextWord.WordText, Pronunciation = nextWord.Pronunciation,
            Definition = nextWord.Definition, PrimaryDefinition = nextWord.PrimaryDefinition
        });
        var answer = new AnswerDetail { WordId = nextWord.Id, Word = nextWord.WordText };

        return new GetNextWordResp(StatusCodes.Status200OK, SuccessMessages.Controller.StudySession.GetNextWordSuccess,
            new NextWordDetail
            {
                Options = options,
                Answer = answer,
                ReviewingWordCount = reviewingWordCount,
                StudyingWordCount = studyingWordCount,
                ExampleSentence = nextWord.ExampleSentence
            });
    }

    public static GetNextWordResp Fail(int code, string message)
    {
        return new GetNextWordResp(code, message, null);
    }
}

# endregion