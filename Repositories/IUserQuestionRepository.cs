using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public interface IUserQuestionRepository : IBaseRepository<UserQuestionEntity>
{
    Task<List<UserQuestionEntity>> GetByUserIdAndQuestionType(Guid userId, ChallengeType questionType);
    Task<List<UserQuestionEntity>> GetByUserId(Guid userId);
}