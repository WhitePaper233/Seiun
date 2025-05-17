using Seiun.Resources;

namespace Seiun.Models.Responses;

//单词Dto
public class WordDto
{
	public required Guid WordId { get; set; }
	public required string WordText { get; set; }
	public string? Pronunciation { get; set; }
	public required string Definition { get; set; }
	public required string ExampleSentence { get; set; }
	public required List<Guid> DistractorIds { get; set; }
	public required List<string> WordBookName { get; set; }
}

#region WordResponse

public class WordListResponse
{
	public required IEnumerable<WordDto> Words { get; set; }
	public required int TotalWords { get; set; }
}

public sealed class WordListResp(int code, string message, WordListResponse? wordListResponse)
	: BaseRespWithData<WordListResponse>(code, message, wordListResponse)
{
	public static WordListResp Success(WordListResponse wordListResponse) => new(200, SuccessMessages.Controller.Word.GetWordSuccess, wordListResponse);

	public static WordListResp Fail(int code, string message) => new(code, message, null);
}

#endregion
