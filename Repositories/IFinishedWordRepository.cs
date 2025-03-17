using Seiun.Entities;
namespace Seiun.Repositories;

public interface IFinishedWordRepository : IBaseRepository<FinishedWordRecordEntity>
{
	Task<IGrouping<Guid,FinishedWordRecordEntity>?> GetLatestFinishedWordIdAsync(Guid userId);
	Task<List<FinishedWordRecordEntity>?> GetWordsToQuestionAsync(Guid userId);
}