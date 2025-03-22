using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class ArticleLikeRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<ArticleLikeEntity>(dbContext, minioClient), IArticleLikeRepository
{
    public async Task<List<Guid>?> GetArticleListByLikedRecordAsync(Guid userId)
    {
        return await DbContext.ArticleLikes
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.LikedTime)
            .Select(a => a.LikedArticleId)
            .ToListAsync();
    }

    public async Task<ArticleLikeEntity?> GetArticleByLikedRecordAsync(Guid userId, Guid articleId)
    {
        return await DbContext.ArticleLikes
            .Where(a => a.UserId == userId && a.LikedArticleId == articleId)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetUserCountByLikedRecordAsync(Guid articleId)
    {
        return await DbContext.ArticleLikes
            .Where(a => a.LikedArticleId == articleId)
            .CountAsync();
    }
}