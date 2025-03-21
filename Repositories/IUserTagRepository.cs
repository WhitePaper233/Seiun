using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public interface IUserTagRepository : IBaseRepository<UserTagEntity>
{
    Task<List<UserTagEntity>?> GetUserTagOfAllWordBookAsync(Guid userId);
    
    Task<UserTagEntity?> GetTagByUserIdAndWordLevelAsync(Guid userId, Guid wordBookId);
    
    Task<UserTagEntity?> GetCurrentUserTagAsync(Guid userId);
}