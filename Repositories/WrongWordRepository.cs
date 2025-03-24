using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class WrongWordRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<WrongWordRecordEntity>(dbContext, minioClient), IWrongWordRepository
{
    public async Task<List<Guid>?> GetErrorWordIdsByUserIdAsync(Guid userId)
    {
        return await DbContext.WrongWords
            .Where(a => a.UserId == userId)
            .Select(a => a.WordId)
            .ToListAsync();
    }

    public void BulkDelete(List<Guid> reviewingWordIds)
    {
        var wordsToDelete = DbContext.WrongWords
            .Where(w => reviewingWordIds.Contains(w.WordId))
            .ToList();

        DbContext.WrongWords.RemoveRange(wordsToDelete);
    }
}