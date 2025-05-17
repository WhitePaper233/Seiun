using System.Diagnostics;
using Seiun.Utils.Enums;

namespace Seiun.Services;

public class CurrentGenerateTaskService(ILogger<CurrentGenerateTaskService> logger) : ICurrentGenerateTaskService
{
	private readonly HashSet<Guid> _currentGenerateAiArticleTaskUserId = [];
	private readonly HashSet<Guid> _currentGenerateClozeTestUserId = [];
	private readonly Lock _lock = new();

	public bool InsertUserId(Guid userId, TaskType taskType)
	{
		lock (_lock)
		{
			var currentTask = taskType switch
			{
				TaskType.AiArticle => _currentGenerateAiArticleTaskUserId,
				TaskType.Challenge => _currentGenerateClozeTestUserId,
				_ => null
			};

			var status = currentTask?.Add(userId) ?? false;
			return status;
		}
	}

	public void DeleteUserId(Guid userId, TaskType taskType)
	{
		lock (_lock)
		{
			var status = taskType switch
			{
				TaskType.AiArticle => _currentGenerateAiArticleTaskUserId.Remove(userId),
				TaskType.Challenge => _currentGenerateClozeTestUserId.Remove(userId),
				_ => false
			};
			if (!status) logger.LogWarning("User {} does not exist in CurrentGenerateTask", userId);
		}
	}
}
