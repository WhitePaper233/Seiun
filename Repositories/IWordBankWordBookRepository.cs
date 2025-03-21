using Seiun.Entities;

namespace Seiun.Repositories;

public interface IWordBankWordBookRepository : IBaseRepository<WordBankWordBookEntity>
{
    Task<List<WordEntity>?> GetWordBookByTagAsync(Guid bookId, int dailyPlan, Guid userId);
}