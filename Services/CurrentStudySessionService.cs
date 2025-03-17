using Seiun.Entities;
using Seiun.Repositories;
using Seiun.Controllers;

namespace Seiun.Services;

public class CurrentStudySessionService : ICurrentStudySessionService
{
	private readonly Dictionary<Guid,Queue<WordEntity>> _CurrentStudySessions = [];

	// 添加Session
    public bool AddSession(Guid SessionId, Queue<WordEntity> Words, ILogger<WordSessionController> logger)
	{
		if(_CurrentStudySessions.ContainsKey(SessionId))
		{
			logger.LogWarning("Session {} already exists",SessionId);
			return false;
		}
		_CurrentStudySessions.Add(SessionId,Words);
		return true;
	}

    // 获取下一个单词
	public WordEntity? GetNextWord(Guid SessionId, ILogger<WordSessionController> logger)
	{
		if(_CurrentStudySessions.ContainsKey(SessionId)==false)
		{
			logger.LogWarning("Session {} does not exist",SessionId);
			return null;
		}
		return _CurrentStudySessions[SessionId].Peek();
	}

	// 删除正确单词
	public void DeleteCorrectWord(Guid SessionId, ILogger<WordSessionController> logger)
	{
		if(_CurrentStudySessions.ContainsKey(SessionId)==false)
		{	
			logger.LogWarning("Session {} does not exist",SessionId);
			return;
		}
		_CurrentStudySessions[SessionId].Dequeue();
	}

    // 插入错误单词到队尾
	public void InsertErrorWord(Guid SessionId, ILogger<WordSessionController> logger)
	{
		if(_CurrentStudySessions.ContainsKey(SessionId)==false)
		{	
			logger.LogWarning("Session {} does not exist",SessionId);
			return;
		}
		var Word = _CurrentStudySessions[SessionId].Dequeue();
		_CurrentStudySessions[SessionId].Enqueue(Word);
	}

    // 会话结束，移除Session
	public void RemoveSession(Guid SessionId, ILogger<WordSessionController> logger)
	{
		if(_CurrentStudySessions.ContainsKey(SessionId)&&_CurrentStudySessions[SessionId].Count==0)
		{
			_CurrentStudySessions.Remove(SessionId);
		}
		else
		{
			logger.LogWarning("Session {} does not exist or not finished",SessionId);
		}
	}

	// 定时清理Session
	public async Task ClearSessionAsync(IWordSessionRepository sessionRepository, ILogger logger)
	{
		var endTime = DateTime.Now;
		var clearingSessions = new List<Guid>();
		foreach(var Session in _CurrentStudySessions)
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
			_CurrentStudySessions.Remove(SessionId);
		}

		if(await sessionRepository.SaveAsync())
		{
			logger.LogInformation("Session cleared successfully");
		}
	}
	
}