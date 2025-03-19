using Seiun.Entities;

namespace Seiun.Repositories;

public interface IFillInBlankWordRepository : IBaseRepository<FillInBlankWordEntity>
{
    void BulkAdd(List<FillInBlankWordEntity> words);
    
    Task<List<FillInBlankWordEntity>?> GetByQuestionIdAsync(Guid questionId);
}