using Seiun.Entities;

namespace Seiun.Repositories;

public interface IAiArticleRepository : IBaseRepository<AiArticleEntity>
{
    Task<List<AiArticleEntity>?> GetByUserIdAsync(Guid userId);
}