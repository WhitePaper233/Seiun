
using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class PublicAnnouncementRepository(SeiunDbContext dbContext, IMinioClient minioClient)
	: BaseRepository<PublicAnnouncementEntity>(dbContext, minioClient), IPublicAnnouncementRepository
{
	public async Task<List<PublicAnnouncementEntity>?> GetRecentlyPublicAnnouncement()
	{
		var sevenDaysAgo = DateTimeOffset.UtcNow.AddDays(-7);

		return await DbContext.PublicAnnouncements
		.Where(a => a.CreatedAt >= sevenDaysAgo)
		.OrderByDescending(a => a.CreatedAt)
		.ToListAsync();
	}
}