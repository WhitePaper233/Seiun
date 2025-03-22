using Seiun.Entities;

namespace Seiun.Repositories;

public interface IWordWordBookRepository : IBaseRepository<WordWordBookEntity>
{
    Task<List<WordEntity>?> GetWordBookByTagAsync(Guid bookId, int dailyPlan, Guid userId);

    // 计算单词数量
    int QueryWordCount(Guid userBookId);
    Task<int> QueryWordCountAsync(Guid userBookId);
}