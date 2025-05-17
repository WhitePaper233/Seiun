using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class AiArticleRepository(SeiunDbContext dbContext, IMinioClient minioClient)
	: BaseRepository<AiArticleEntity>(dbContext, minioClient), IAiArticleRepository
{
	public async Task<List<Guid>?> GetListByUserIdAsync(Guid userId)
	{
		return await DbContext.AiArticles
			.Where(a => a.UserId == userId)
			.OrderByDescending(a => a.CreatedAt)
			.Select(a => a.Id)
			.ToListAsync();
	}
}
