using Seiun.Entities;

namespace Seiun.Repositories;

public interface IArticleLikeRepository : IBaseRepository<ArticleLikeEntity>
{
    Task<List<Guid>?> GetArticleListByLikedRecordAsync(Guid userId);
    Task<ArticleLikeEntity?> GetArticleByLikedRecordAsync(Guid userId, Guid articleId);
    Task<int> GetUserCountByLikedRecordAsync(Guid articleId);
}