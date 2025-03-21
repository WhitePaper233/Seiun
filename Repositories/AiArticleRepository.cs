using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class AIArticleRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<AiArticleEntity>(dbContext, minioClient), IAIArticleRepository
{
    public async Task<List<AiArticleEntity>?> GetByUserIdAsync(Guid userId)
    {
        return await DbContext.AiArticles
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .ToListAsync();
    }
}