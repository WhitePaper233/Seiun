using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public class UserTagRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<UserTagEntity>(dbContext, minioClient), IUserTagRepository
{
    public async Task<List<UserTagEntity>?> GetUserTagOfAllWordBankAsync(Guid userId)
    {
        return await DbContext.UserTag
            .Where(u => u.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserTagEntity?> GetTagByUserIdAndWordLevelAsync(Guid userId, WordLevel wordLevel)
    {
        return await DbContext.UserTag
            .Where(w => w.UserId == userId && w.WordLevel == wordLevel)
            .FirstOrDefaultAsync();
    }

    public Task<UserTagEntity?> GetCurrentUserTagAsync(Guid userId)
    {
        return DbContext.UserTag
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.SettingAt)
            .FirstOrDefaultAsync();
    }
}
