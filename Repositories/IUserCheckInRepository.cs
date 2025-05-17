using Seiun.Entities;

namespace Seiun.Repositories;

public interface IUserCheckInRepository : IBaseRepository<UserCheckInEntity>
{
	Task<UserCheckInEntity?> LastCheckInAsync(Guid userId);
	Task<List<DateTimeOffset>> GetUserAllCheckInsAsync(Guid userId);
	Task<Dictionary<Guid, DateTimeOffset?>> GetLastCheckInTimesAsync(List<Guid> userIds);
}
