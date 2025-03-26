using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public interface IMistakeBookRepository : IBaseRepository<MistakeBookEntity>
{
    Task<List<Guid>?> GetByStatus(MistakeStatus mistakeStatus, Guid userId);
}