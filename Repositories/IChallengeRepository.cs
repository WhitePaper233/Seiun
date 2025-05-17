using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public interface IChallengeRepository : IBaseRepository<ChallengeEntity>
{
	Task<List<Guid>?> GetByUserId(Guid userId, ChallengeType challengeType);
	Task<List<Guid>?> GetByUserId(Guid userId);
}
