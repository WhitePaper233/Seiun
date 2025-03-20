using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public class WordRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    :BaseRepository<WordEntity>(dbContext,minioClient), IWordRepository
{
    public async Task<List<WordEntity>?> GetWordsByTagAsync(WordLevel tagName, Guid userId, int num)
    {
        var finishedWordIds = await DbContext.FinishedWords
            .Where(a => a.UserId == userId)
            .Select(f => f.WordId) // 只获取需要的字段，提高性能
            .ToListAsync();

        return await DbContext.Words
            .Where(w => w.Tag == tagName)
            .Where(w => !finishedWordIds.Contains(w.Id)) // 在数据库中过滤
            .Include(w => w.WordDistractors)
            .Take(num)
            .ToListAsync();
    } 
}
