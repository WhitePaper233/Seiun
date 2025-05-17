using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public interface IMistakeBookRepository : IBaseRepository<MistakeBookEntity>
{
	Task<List<Guid>?> GetByStatus(Guid userId);
	Task<List<WordEntity>> GetMistakeWordDetailsByUserId(Guid userId);
}
