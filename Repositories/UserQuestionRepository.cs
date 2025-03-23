using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public class UserQuestionRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<UserQuestionEntity>(dbContext, minioClient), IUserQuestionRepository
{
    public async Task<List<UserQuestionEntity>> GetByUserIdAndQuestionType(Guid userId, ChallengeType questionType)
    {
        return await DbContext.UserQuestions
            .Where(u => u.UserId == userId && u.Type == questionType)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<UserQuestionEntity>> GetByUserId(Guid userId)
    {
        return await DbContext.UserQuestions
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }
}