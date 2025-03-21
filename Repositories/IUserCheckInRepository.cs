using Seiun.Entities;

namespace Seiun.Repositories;

public interface IUserCheckInRepository : IBaseRepository<UserCheckInEntity>
{
    Task<UserCheckInEntity?> LastCheckInAsync(Guid userId);
    Task<List<UserCheckInEntity>> GetUserCheckInsAsync(Guid userId);
    Task<DateTime> GetLastCheckInTimeAsync(Guid userId);
    Task<Dictionary<Guid, DateTime?>> GetLastCheckInTimesAsync(List<Guid> userIds);
}