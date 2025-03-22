using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class WordWordBookRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<WordWordBookEntity>(dbContext, minioClient), IWordWordBookRepository
{
    public async Task<List<WordEntity>?> GetUnfinishedWordsByPlanAsync(Guid bookId, int dailyPlan, Guid userId)
    {
        var finishedWordIds = await DbContext.FinishedWords
            .Where(a => a.UserId == userId)
            .Select(f => f.WordId)
            .ToListAsync();

        return await DbContext.WordWordBooks
            .Where(w => w.BookId == bookId)
            .Where(w => !finishedWordIds.Contains(w.WordId))
            .Take(dailyPlan)
            .Include(w => w.Word.WordDistractors)
            .Select(w => w.Word)
            .ToListAsync();
    }

    // 计算单词数量
    public int GetBookWordCount(Guid userBookId)
    {
        return DbContext.WordWordBooks.Count(entity => entity.BookId == userBookId);
    }

    public async Task<int> GetBookWordCountAsync(Guid userBookId)
    {
        return await DbContext.WordWordBooks.CountAsync(entity => entity.BookId == userBookId);
    }
}