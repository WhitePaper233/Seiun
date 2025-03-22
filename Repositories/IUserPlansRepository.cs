using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public interface IUserPlansRepository : IBaseRepository<UserPlanEntity>
{
    Task<List<UserPlanEntity>?> GetAllUserPlannedWordBooksAsync(Guid userId);

    Task<UserPlanEntity?> GetUserPlanAsync(Guid userId, Guid wordBookId);

    Task<UserPlanEntity?> GetUserPlanAsync(Guid userId);
}