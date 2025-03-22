using Seiun.Entities;
using Seiun.Resources;

namespace Seiun.Models.Responses;

//单词Dto
public class WordDto
{
	public required Guid WordId { get; set; }
	public required string WordText { get; set; }
	public string? Pronunciation { get; set; }
	public required string Definition { get; set; }
	public required List<Guid> DistractorIds { get; set; }
	public required List<string> WordBookName { get; set; }
}

# region ReviewingWordListResponse

public class ReviewingWords
{
	public required IEnumerable<WordEntity> Words { get; set; }
}

public sealed class ReviewingWordListResp(int code, string message, ReviewingWords? reviewingWords)
	: BaseRespWithData<ReviewingWords>(code, message, reviewingWords)
{
	public static ReviewingWordListResp Success(IEnumerable<WordEntity> reviewingWords)
	{
		return new ReviewingWordListResp(StatusCodes.Status200OK, SuccessMessages.Controller.Word.GetReviewingWordSuccess, 
		new ReviewingWords
		{
			Words = reviewingWords
		});
	}

	public static ReviewingWordListResp Fail(int code, string message)
	{
		return new ReviewingWordListResp(code, message, null);
	}
}

# endregion

#region WordResponse
public class WordListResponse
{
	public required IEnumerable<WordDto> Words { get; set; }
	public required int TotalWords { get; set; }
}

public sealed class WordListResp(int code, string message, WordListResponse? wordListResponse)
	: BaseRespWithData<WordListResponse>(code, message, wordListResponse)
{
	public static WordListResp Success(WordListResponse wordListResponse)
    {
        return new WordListResp(200, SuccessMessages.Controller.Word.GetWordSuccess, wordListResponse);
    }

	public static WordListResp Fail(int code, string message)
	{
		return new WordListResp(code, message, null);
	}
}

#endregion