using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class WordSessionRepository(SeiunDbContext dbContext, IMinioClient minioClient)
	: BaseRepository<WordSessionEntity>(dbContext, minioClient), IWordSessionRepository
{
	public Task<WordSessionEntity?> GetSessionByUserIdAsync(Guid userId)
	{
		return DbContext.Sessions.Where(s => s.UserId == userId).FirstOrDefaultAsync();
	}
}