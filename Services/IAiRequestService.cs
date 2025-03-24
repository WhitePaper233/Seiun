namespace Seiun.Services;

public interface IAiRequestService
{
    Task GenerateAiArticleAsync(Guid userId);
    Task GenerateAiFillInBlankAsync(List<string> words, Guid userId, Guid sessionId);
    Task GenerateAiClozeTest(List<string> words, Guid userId, Guid sessionId);
}