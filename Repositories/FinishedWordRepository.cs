using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class FinishedWordRepository(SeiunDbContext dbContext, IMinioClient minioClient)
	: BaseRepository<FinishedWordRecordEntity>(dbContext, minioClient), IFinishedWordRepository
{
    public async Task<IGrouping<Guid,FinishedWordRecordEntity>?> GetLatestFinishedWordIdAsync(Guid userId)
	{	
		var LatestFinishedWords = await DbContext.FinishedWords
			.Where(x => x.UserId == userId)
			.OrderByDescending(x => x.FinishedAt)
			.ToListAsync();
		
		return LatestFinishedWords
			.GroupBy(x => x.SessionId)
			.FirstOrDefault();
	}

	public async Task<List<FinishedWordRecordEntity>?> GetWordsToQuestionAsync(Guid userId)
	{
		return await DbContext.FinishedWords
			.Where(w => w.UserId == userId)
			.OrderByDescending(w => w.Id)
			.Take(15)
			.ToListAsync();
	}
}