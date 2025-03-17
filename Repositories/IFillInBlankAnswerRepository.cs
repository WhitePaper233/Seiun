using Seiun.Entities;

namespace Seiun.Repositories;

public interface IFillInBlankAnswerRepository : IBaseRepository<FillInBlankAnswerEntity>
{ 
    void BulkAdd(List<FillInBlankAnswerEntity> answers);
}