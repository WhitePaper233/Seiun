

namespace Seiun.Services;

public interface IAiRequestService
{	
	Task GenerateAiArticleAsync(Guid userId);
	Task GenerateAiFillInBlankAsync(List<string> words, Guid userId);
	Task GenerateAiClozeTest(List<string> words, Guid userId);
}