using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public class UserChallengeRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<UserChallengeEntity>(dbContext, minioClient), IUserChallengeRepository
{
    public async Task<List<UserChallengeEntity>> GetByUserIdAndQuestionType(Guid userId, ChallengeType questionType)
    {
        return await DbContext.UserQuestions
            .Where(u => u.UserId == userId && u.Type == questionType)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<UserChallengeEntity>> GetByUserId(Guid userId)
    {
        return await DbContext.UserQuestions
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }
}