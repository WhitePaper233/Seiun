using Seiun.Entities;

namespace Seiun.Repositories;

public interface IWordSessionRepository : IBaseRepository<WordSessionEntity>
{
	Task<WordSessionEntity?> GetSessionByUserIdAsync(Guid userId);
	Task<List<WordSessionEntity>?> GetTodayAllSessionsByUserIdAsync(Guid userId);
	Task<WordSessionEntity?> GetChallengeByIdAsync(Guid sessionId);
}
