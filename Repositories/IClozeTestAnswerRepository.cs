using Seiun.Entities;

namespace Seiun.Repositories;

public interface IClozeTestAnswerRepository : IBaseRepository<ClozeTestAnswerEntity>
{
    void BulkAdd(List<ClozeTestAnswerEntity> answers);
    Task<List<ClozeTestAnswerEntity>?> GetByQuestionIdAsync(Guid questionId);
}