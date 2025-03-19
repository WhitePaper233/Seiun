using Seiun.Entities;
using Seiun.Repositories;
using Seiun.Controllers;

namespace Seiun.Services;

public class CurrentStudySessionService : ICurrentStudySessionService
{
	private Dictionary<Guid,Queue<WordEntity>> _currentStudySessions = [];

	// 添加Session
    public bool AddSession(Guid SessionId, Queue<WordEntity> Words, ILogger<WordSessionController> logger)
	{
		if(_currentStudySessions.ContainsKey(SessionId))
		{
			logger.LogWarning("Session {} already exists",SessionId);
			return false;
		}
		_currentStudySessions.Add(SessionId,Words);
		return true;
	}

    // 获取下一个单词
	public WordEntity? GetNextWord(Guid SessionId, ILogger<WordSessionController> logger)
	{
		if(_currentStudySessions.ContainsKey(SessionId))
		{
			return _currentStudySessions[SessionId].Count == 0 ? null : _currentStudySessions[SessionId].Peek();
		}
		
		logger.LogWarning("Session {} does not exist",SessionId);
		return null;
	}

	// 删除正确单词
	public void DeleteCorrectWord(Guid SessionId, ILogger<WordSessionController> logger)
	{
		if(_currentStudySessions.ContainsKey(SessionId)==false)
		{	
			logger.LogWarning("Session {} does not exist",SessionId);
			return;
		}
		_currentStudySessions[SessionId].Dequeue();
	}

    // 插入错误单词到队尾
	public void InsertErrorWord(Guid SessionId, ILogger<WordSessionController> logger)
	{
		if(_currentStudySessions.ContainsKey(SessionId)==false)
		{	
			logger.LogWarning("Session {} does not exist",SessionId);
			return;
		}
		var Word = _currentStudySessions[SessionId].Dequeue();
		_currentStudySessions[SessionId].Enqueue(Word);
	}

    // 会话结束，移除Session
	public void RemoveSession(Guid SessionId, ILogger<WordSessionController> logger)
	{
		if(_currentStudySessions.ContainsKey(SessionId)&&_currentStudySessions[SessionId].Count==0)
		{
			_currentStudySessions.Remove(SessionId);
		}
		else
		{
			logger.LogWarning("Session {} does not exist or not finished",SessionId);
		}
	}

	// 定时清理Session
	public async Task ClearSessionAsync(IWordSessionRepository sessionRepository, ILogger logger)
	{
		var endTime = DateTimeOffset.UtcNow;
		var clearingSessions = new List<Guid>();
		foreach(var Session in _currentStudySessions)
		{
			try
			{
				var userSession = await sessionRepository.GetByIdAsync(Session.Key);
				if(userSession!=null)
				{
					TimeSpan hoursSpan = endTime - userSession.WordSessionAt;
					if(hoursSpan.TotalHours>20)
					{
						clearingSessions.Add(Session.Key);
						sessionRepository.Delete(userSession);
					}
				}
				else
				{
					logger.LogWarning("SessionEntity does not exist for Session {}",Session.Key);
				}
			}
			catch(Exception e)
			{
				logger.LogWarning(e,"Error when clearing session {}",Session.Key);
				continue;
			}
		}
		
		foreach(var SessionId in clearingSessions)
		{
			_currentStudySessions.Remove(SessionId);
		}

		if(await sessionRepository.SaveAsync())
		{
			logger.LogInformation("Session cleared successfully");
		}
	}
	
}