using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public interface  IWordRepository : IBaseRepository<WordEntity>
{
    Task<List<WordEntity>?> GetWordsByTagAsync(WordLevel tagName, Guid userId, int num);    
}