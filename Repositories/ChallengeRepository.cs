using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public class ChallengeRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<ChallengeEntity>(dbContext, minioClient), IChallengeRepository
{
    public async Task<List<Guid>?> GetByUserId(Guid userId, ChallengeType challengeType)
    {
        return await DbContext.Challenges
            .Where(c => c.UserId == userId)
            .Where(c => c.Type == challengeType)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.Id)
            .ToListAsync();
    }

    public async Task<List<Guid>?> GetByUserId(Guid userId)
    {
        return await DbContext.Challenges
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.Id)
            .ToListAsync();
    }
}