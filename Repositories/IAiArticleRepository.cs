using Seiun.Entities;

namespace Seiun.Repositories;

public interface IAiArticleRepository : IBaseRepository<AiArticleEntity>
{
    Task<List<Guid>?> GetListByUserIdAsync(Guid userId);
}