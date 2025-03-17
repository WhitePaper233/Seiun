using Seiun.Entities;

namespace Seiun.Repositories;

public interface IAIArticleRepository : IBaseRepository<AiArticleEntity>
{
	Task<List<AiArticleEntity>?> GetByUserIdAsync(Guid userId);
}