using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class FinishedWordRepository(SeiunDbContext dbContext, IMinioClient minioClient)
	: BaseRepository<FinishedWordRecordEntity>(dbContext, minioClient), IFinishedWordRepository
{
	public async Task<IGrouping<Guid, FinishedWordRecordEntity>?> GetLatestFinishedWordIdAsync(Guid userId)
	{
		var latestFinishedWords = await DbContext.FinishedWords
			.Where(x => x.UserId == userId)
			.OrderByDescending(x => x.CreatedAt)
			.ToListAsync();

		return latestFinishedWords
			.GroupBy(x => x.SessionId)
			.FirstOrDefault();
	}

	public async Task<int> GetLearnedCountAsync(Guid userId, Guid wordBookId)
	{
		return await DbContext.FinishedWords
			.Join(DbContext.WordWordBooks,
			outerKeySelector: word => word.WordId,
			innerKeySelector: wordWordBook => wordWordBook.WordId,
			resultSelector: (word, wordWordBook) => new { word, wordWordBook })
			.Where(joined => joined.word.UserId == userId && joined.wordWordBook.BookId == wordBookId)
			.CountAsync();
	}

	public int GetLearnedCount(Guid userId, Guid wordBookId)
	{
		return DbContext.FinishedWords
			.Join(DbContext.WordWordBooks,
			outerKeySelector: word => word.WordId,
			innerKeySelector: wordWordBook => wordWordBook.WordId,
			resultSelector: (word, wordWordBook) => new { word, wordWordBook })
			.Count(joined => joined.word.UserId == userId && joined.wordWordBook.BookId == wordBookId);
	}
}
