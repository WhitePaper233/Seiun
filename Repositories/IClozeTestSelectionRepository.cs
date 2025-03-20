using Seiun.Entities;

namespace Seiun.Repositories;

public interface IClozeTestSelectionRepository : IBaseRepository<ClozeTestSelectionEntity>
{
    void BulkAdd(List<ClozeTestSelectionEntity> selections);
    Task<List<ClozeTestSelectionEntity>?> GetByQuestionIdAsync(Guid questionId);
}