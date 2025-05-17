using Seiun.Utils.Enums;

namespace Seiun.Services;

public interface ICurrentGenerateTaskService
{
	bool InsertUserId(Guid userId, TaskType taskType);
	void DeleteUserId(Guid userId, TaskType taskType);
}
