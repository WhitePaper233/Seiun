using Seiun.Models.Responses;

namespace Seiun.Services;

public interface IAIRequestService
{	
	Task<string?> GetAIArticleAsync(List<string> words);
	Task<string?> GetAICoverAsync(string aiArticle);
	Task<FillInBlankInfo?> GetAiFillInBlankAsync(List<string> words);
}