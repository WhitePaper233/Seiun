using Seiun.Entities;

namespace Seiun.Repositories;

public interface IWordWordBookRepository : IBaseRepository<WordWordBookEntity>
{
	Task<List<WordEntity>?> GetUnfinishedWordsByPlanAsync(Guid bookId, int dailyPlan, Guid userId);

	// 计算单词数量
	int GetBookWordCount(Guid userBookId);
	Task<int> GetBookWordCountAsync(Guid userBookId);
}
