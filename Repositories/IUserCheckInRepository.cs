using Seiun.Entities;

namespace Seiun.Repositories;

public interface IUserCheckInRepository : IBaseRepository<UserCheckInEntity>
{
    Task<bool> CheckInTodayAsync(Guid userId);
    Task<List<UserCheckInEntity>> GetUserCheckInsAsync(Guid userId);
    Task<DateTimeOffset> GetLastCheckInTimeAsync(Guid userId);
    Task<Dictionary<Guid, DateTimeOffset?>> GetLastCheckInTimesAsync(List<Guid> userIds);
}