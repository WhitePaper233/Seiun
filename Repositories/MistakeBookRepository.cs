using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class MistakeBookRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<MistakeBookEntity>(dbContext, minioClient), IMistakeBookRepository
{
    public async Task<List<Guid>?> GetByStatus(Guid userId)
    {
        return await DbContext.MistakeBook
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => m.WordId)
            .ToListAsync();
    }

    public async Task<List<WordEntity>> GetMistakeWordDetailsByUserId(Guid userId)
    {
        return await (from m in DbContext.MistakeBook
            join w in DbContext.Words on m.WordId equals w.Id
            where m.UserId == userId
            select w).ToListAsync();
    }
}