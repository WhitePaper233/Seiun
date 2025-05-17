using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public class UserPlansRepository(SeiunDbContext dbContext, IMinioClient minioClient)
	: BaseRepository<UserPlanEntity>(dbContext, minioClient), IUserPlansRepository
{
	public async Task<List<UserPlanEntity>?> GetAllUserPlannedWordBooksAsync(Guid userId)
	{
		return await DbContext.UserPlans
			.Where(u => u.UserId == userId)
			.ToListAsync();
	}

	public async Task<UserPlanEntity?> GetUserPlanAsync(Guid userId, Guid wordBookId)
	{
		return await DbContext.UserPlans
			.Where(w => w.UserId == userId && w.WordBookId == wordBookId)
			.FirstOrDefaultAsync();
	}

	public Task<UserPlanEntity?> GetUserPlanAsync(Guid userId)
	{
		return DbContext.UserPlans
			.Where(w => w.UserId == userId)
			.OrderByDescending(w => w.UpdatedAt)
			.FirstOrDefaultAsync();
	}
}
