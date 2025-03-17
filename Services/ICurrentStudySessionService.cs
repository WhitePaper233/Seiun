using Seiun.Entities;
using Seiun.Repositories;
using Seiun.Controllers;

namespace Seiun.Services;

public interface ICurrentStudySessionService
{
	public bool AddSession(Guid SessionId, Queue<WordEntity> Words, ILogger<WordSessionController> logger);
	public WordEntity? GetNextWord(Guid SessionId, ILogger<WordSessionController> logger);
	public void DeleteCorrectWord(Guid SessionId, ILogger<WordSessionController> logger);
	public void InsertErrorWord(Guid SessionId, ILogger<WordSessionController> logger);
	public void RemoveSession(Guid SessionId, ILogger<WordSessionController> logger);
	public Task ClearSessionAsync(IWordSessionRepository sessionRepository, ILogger logger);
}