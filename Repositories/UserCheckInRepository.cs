using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class UserCheckInRepository(SeiunDbContext dbContext, IMinioClient minioClient)
	: BaseRepository<UserCheckInEntity>(dbContext, minioClient), IUserCheckInRepository
{
    public async Task<UserCheckInEntity?> LastCheckInAsync(Guid userId)
    {
        return await DbContext.UserCheckIns
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CheckInAt)
            .FirstOrDefaultAsync();
    }
    
    // 获取用户的所有打卡记录，并按日期降序排列
    public async Task<List<DateTimeOffset>> GetUserAllCheckInsAsync(Guid userId)
    {
        return await DbContext.UserCheckIns
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.CreatedAt)
            .ToListAsync();
    }

    // 获取多个用户的最后一次打卡时间
    public async Task<Dictionary<Guid, DateTimeOffset?>> GetLastCheckInTimesAsync(List<Guid> userIds)
    {
        return await DbContext.UserCheckIns
            .Where(x => userIds.Contains(x.UserId))
            .GroupBy(x => x.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                LastCheckInTime = g.Max(x => x.CheckInAt) 
            })
            .ToDictionaryAsync(x => x.UserId, x => (DateTimeOffset?)x.LastCheckInTime);
    }

}