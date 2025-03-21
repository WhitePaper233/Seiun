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
            .OrderByDescending(x => x.CheckInDate)
            .FirstOrDefaultAsync();
    }
    // Todo

    // 获取用户的所有打卡记录，并按日期降序排列
    public async Task<List<UserCheckInEntity>> GetUserCheckInsAsync(Guid userId)
    {
        return await DbContext.UserCheckIns
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CheckInDate)  
            .ToListAsync();
    }
    
    // 获取用户最后一次打卡时间
    public async Task<DateTimeOffset> GetLastCheckInTimeAsync(Guid userId)
    {
        return await DbContext.UserCheckIns
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CheckInDate)
            .Select(x => x.CheckInDate)
            .FirstOrDefaultAsync();
    }

    // 获取多个用户的最后一次打卡时间
    public async Task<Dictionary<Guid, DateTime?>> GetLastCheckInTimesAsync(List<Guid> userIds)
    {
        return await DbContext.UserCheckIns
            .Where(x => userIds.Contains(x.UserId))
            .GroupBy(x => x.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                LastCheckInTime = g.Max(x => x.CheckInDate) 
            })
            .ToDictionaryAsync(x => x.UserId, x => (DateTime?)x.LastCheckInTime);
    }

}