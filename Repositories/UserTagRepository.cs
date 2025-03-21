using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public class UserTagRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<UserTagEntity>(dbContext, minioClient), IUserTagRepository
{
    public async Task<List<UserTagEntity>?> GetUserTagOfAllWordBookAsync(Guid userId)
    {
        return await DbContext.UserTag
            .Where(u => u.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserTagEntity?> GetTagByUserIdAndWordLevelAsync(Guid userId, Guid wordBookId)
    {
        return await DbContext.UserTag
            .Where(w => w.UserId == userId && w.WordBookId == wordBookId)
            .FirstOrDefaultAsync();
    }

    public Task<UserTagEntity?> GetCurrentUserTagAsync(Guid userId)
    {
        return DbContext.UserTag
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.UpdatedAt)
            .FirstOrDefaultAsync();
    }
}
