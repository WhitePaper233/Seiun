using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class ErrorWordRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<ErrorWordRecordEntity>(dbContext, minioClient), IErrorWordRepository
{
    public async Task<List<Guid>?> GetErrorWordIdsByUserIdAsync(Guid userId)
    {
        return await DbContext.ErrorWords
            .Where(a => a.UserId == userId)
            .Select(a => a.WordId)
            .ToListAsync();
    }

    public void BulkDelete(List<Guid> reviewingWordIds)
    {
        var wordsToDelete = DbContext.ErrorWords
            .Where(w => reviewingWordIds.Contains(w.WordId))
            .ToList();

        DbContext.ErrorWords.RemoveRange(wordsToDelete);
    }
}