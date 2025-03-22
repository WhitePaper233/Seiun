using Seiun.Entities;

namespace Seiun.Services;

public class CurrentStudySessionService(IServiceScopeFactory serviceScopeFactory, ILogger<CurrentStudySessionService> logger) : ICurrentStudySessionService
{
	private  readonly Dictionary<Guid,Queue<WordEntity>> _currentStudySessions = [];
	
	// 添加Session
    public bool AddSession(Guid sessionId, Queue<WordEntity> words)
	{
		if (_currentStudySessions.TryAdd(sessionId, words))
		{
			return true;
		}
		logger.LogWarning("Session {} already exists", sessionId);
		return false;
	}

    // 获取下一个单词
	public WordEntity? GetNextWord(Guid sessionId)
	{
		if(_currentStudySessions.ContainsKey(sessionId))
		{
			return _currentStudySessions[sessionId].Count == 0 ? null : _currentStudySessions[sessionId].Peek();
		}
		
		logger.LogWarning("Session {} does not exist",sessionId);
		return null;
	}

	// 删除正确单词
	public void DeleteCorrectWord(Guid sessionId)
	{
		if(!_currentStudySessions.TryGetValue(sessionId, out _))
		{	
			logger.LogWarning("Session {} does not exist",sessionId);
			return;
		}
		_currentStudySessions[sessionId].Dequeue();
	}

    // 插入错误单词到队尾
	public void InsertErrorWord(Guid sessionId)
	{
		if(!_currentStudySessions.TryGetValue(sessionId, out _))
		{	
			logger.LogWarning("Session {} does not exist",sessionId);
			return;
		}
		var word = _currentStudySessions[sessionId].Dequeue();
		_currentStudySessions[sessionId].Enqueue(word);
	}

	public Queue<WordEntity>? GetAllWords(Guid sessionId)
	{
		if(_currentStudySessions.TryGetValue(sessionId, out var words))
		{	
			return words;
		}
		logger.LogWarning("Session {} does not exist",sessionId);
		return null;
	}

	// 会话结束，移除Session
	public void RemoveSession(Guid sessionId)
	{
		if(_currentStudySessions.TryGetValue(sessionId, out _) && _currentStudySessions[sessionId].Count == 0)
		{
			_currentStudySessions.Remove(sessionId);
		}
		else
		{
			logger.LogWarning("Session {} does not exist or not finished",sessionId);
		}
	}

	// 定时清理Session
	public async Task ClearSessionAsync()
	{
		using var scope = serviceScopeFactory.CreateScope();
		var repository = scope.ServiceProvider.GetRequiredService<IRepositoryService>();
		
		var endTime = DateTimeOffset.UtcNow;
		var clearingSessionIds = new List<Guid>();
		foreach(var session in _currentStudySessions)
		{
			var userSession = await repository.SessionRepository.GetByIdAsync(session.Key);
			if(userSession == null)
			{
				logger.LogWarning("SessionEntity does not exist for Session {}",session.Key);
				continue;
			}
			var hoursSpan = endTime - userSession.CreatedAt;
			if(hoursSpan.TotalHours<=20)
			{
				continue;
			}
			clearingSessionIds.Add(session.Key);
		}
		
		foreach(var sessionId in clearingSessionIds)
		{
			_currentStudySessions.Remove(sessionId);
		}
	}
	
}