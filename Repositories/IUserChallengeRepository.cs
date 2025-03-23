using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public interface IUserChallengeRepository : IBaseRepository<UserChallengeEntity>
{
    Task<List<UserChallengeEntity>> GetByUserIdAndQuestionType(Guid userId, ChallengeType questionType);
    Task<List<UserChallengeEntity>> GetByUserId(Guid userId);
}