using Seiun.Controllers;
using Seiun.Entities;
using Seiun.Models.Responses;

namespace Seiun.Services;

public interface IAiRequestService
{	
	Task GenerateAiArticleAsync(Guid userId, IRepositoryService repository, ILogger<WordSessionController> logger);
	Task GenerateAiFillInBlankAsync(List<string> words, Guid userId, IRepositoryService repository, ILogger<WordSessionController> logger);
	Task GenerateAiClozeTest(List<string> words, Guid userId, IRepositoryService repository, ILogger<WordSessionController> logger);
}