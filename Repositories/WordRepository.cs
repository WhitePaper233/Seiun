using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class WordRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    :BaseRepository<WordEntity>(dbContext,minioClient), IWordRepository
{
    public async Task<List<WordEntity>> GetReviewingWordsByGuidsAsync(List<Guid> reviewingWordIds)
    {
        return await DbContext.Words
            .Where(w => reviewingWordIds.Contains(w.Id))
            .Include(w => w.WordDistractors)
            .ToListAsync();
    }
}
