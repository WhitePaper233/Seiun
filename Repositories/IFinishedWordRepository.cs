using Seiun.Entities;

namespace Seiun.Repositories;

public interface IFinishedWordRepository : IBaseRepository<FinishedWordRecordEntity>
{
	Task<IGrouping<Guid, FinishedWordRecordEntity>?> GetLatestFinishedWordIdAsync(Guid userId);

	Task<int> GetLearnedCountAsync(Guid userId, Guid wordBookId);
	int GetLearnedCount(Guid userId, Guid wordBookId);
}
