using Seiun.Entities;

namespace Seiun.Repositories;

public interface IWrongWordRepository : IBaseRepository<WrongWordRecordEntity>
{
	public Task<List<Guid>?> GetErrorWordIdsByUserIdAsync(Guid userId);
	public void BulkDelete(List<Guid> reviewingWordIds);
}
