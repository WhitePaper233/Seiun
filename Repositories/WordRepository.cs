using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public class WordRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    :BaseRepository<WordEntity>(dbContext,minioClient), IWordRepository
{
    public async Task<List<WordEntity>> GetAllWordsAsync(int index, int size, Guid? keyword)
    {
        var query = DbContext.Words
            .Include(w => w.WordDistractors)
            .Include(w => w.Books)
            .ThenInclude(b => b.Book)
            .AsQueryable();

        if (keyword.HasValue)
        {
            query = query.Where(w => w.Id == keyword.Value);
        }

        return await query
            .Skip((index - 1) * size) 
            .Take(size)
            .ToListAsync();
    }
    public async Task<int> GetTotalWordsAsync(Guid? keyword)
    {
        var query = DbContext.Words.AsQueryable();
        if (keyword.HasValue)
        {
            query = query.Where(w => w.Id == keyword.Value);
        }
        return await query.CountAsync();
    }
}
