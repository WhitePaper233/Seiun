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

    public void BulkDelete(Guid userId)
    {
        DbContext.ErrorWords
            .Where(x => x.UserId == userId)
            .ExecuteDelete();
    }
}