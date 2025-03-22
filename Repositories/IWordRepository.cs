using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public interface  IWordRepository : IBaseRepository<WordEntity>
{
    Task<List<WordEntity>> GetAllWordsAsync(int index, int size, Guid? keyword);
    Task<int> GetTotalWordsAsync(Guid? keyword);
}