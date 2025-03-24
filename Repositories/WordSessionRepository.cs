using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class WordSessionRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<WordSessionEntity>(dbContext, minioClient), IWordSessionRepository
{
    public async Task<WordSessionEntity?> GetSessionByUserIdAsync(Guid userId)
    {
        return await DbContext.Sessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<List<WordSessionEntity>?> GetTodayAllSessionsByUserIdAsync(Guid userIds)
    {
        return await DbContext.Sessions
            .Where(s => s.UserId == userIds)
            .Where(s => s.UpdatedAt.Date == DateTimeOffset.UtcNow.Date)
            .OrderBy(s => s.UpdatedAt)
            .ToListAsync();
    }

    public async Task<WordSessionEntity?> GetChallengeByIdAsync(Guid sessionId)
    {
        return await DbContext.Sessions
            .Where(s => s.Id == sessionId)
            .Include(s => s.Challenges)
            .FirstOrDefaultAsync();
    }
}