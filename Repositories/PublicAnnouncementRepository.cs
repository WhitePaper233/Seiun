
using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class PublicAnnouncementRepository(SeiunDbContext dbContext, IMinioClient minioClient)
	: BaseRepository<PublicAnnouncementEntity>(dbContext, minioClient), IPublicAnnouncementRepository
{
	public async Task<List<PublicAnnouncementEntity>?> GetRecentlyPublicAnnouncement()
	{
		DateTimeOffset sevenDaysAgo = DateTimeOffset.UtcNow.AddDays(-7);

		return await DbContext.PublicAnnouncements
		.Where(a => a.PublishTime >= sevenDaysAgo)
		.OrderByDescending(a => a.PublishTime)
		.ToListAsync();
	}
}