using Seiun.Entities;

namespace Seiun.Repositories;

public interface IWordRepository : IBaseRepository<WordEntity>
{
    Task<List<WordEntity>> GetReviewingWordsByGuidsAsync(List<Guid> reviewingWordIds);
}