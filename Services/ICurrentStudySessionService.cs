using Seiun.Entities;

namespace Seiun.Services;

public interface ICurrentStudySessionService
{
    public bool AddSession(Guid sessionId, Queue<WordEntity> words);
    public WordEntity? GetNextWord(Guid sessionId);
    public void DeleteCorrectWord(Guid sessionId);
    public void InsertErrorWord(Guid sessionId);
    public void RemoveSession(Guid sessionId);
    public Queue<WordEntity>? GetAllWords(Guid sessionId);
    public Task ClearSessionAsync();
}