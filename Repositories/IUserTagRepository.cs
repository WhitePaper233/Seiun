using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public interface IUserTagRepository : IBaseRepository<UserTagEntity>
{
    Task<List<UserTagEntity>?> GetUserTagOfAllWordBankAsync(Guid userId);
    
    Task<UserTagEntity?> GetTagByUserIdAndWordLevelAsync(Guid userId, WordLevel wordLevel);
    
    // Task<UserTagEntity?> GetCurrentUserTagAsync(Guid userId);
}