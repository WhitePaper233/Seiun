using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class WordRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    :BaseRepository<WordEntity>(dbContext,minioClient), IWordRepository
{
    public async void BulkInsert(IEnumerable<WordEntity> words)
    {
        await DbContext.Set<WordEntity>().AddRangeAsync(words);
    }
    public async Task<List<string>> GetExistingWordsAsync(IEnumerable<string> wordTexts)
    {
        return await DbContext.Set<WordEntity>()
            .Where(w => wordTexts.Contains(w.WordText))
            .Select(w => w.WordText)
            .ToListAsync();
    }

    public async Task<List<WordEntity>> GetWordsByTagAsync(string tagName, Guid userId, int num)
    {
        var finishedWords = await DbContext.FinishedWords
            .Where(a => a.UserId == userId)
            .ToListAsync();

        return await DbContext.Words
           .Where(w => w.Tags.Any(t => t.Name == tagName))
           .Where(w => !finishedWords.Any(f => f.WordId == w.Id))
           .Include(w => w.Tags)
           .Include(w => w.Distractors)
           .Take(num)
           .ToListAsync();
    } 
}

public class TagRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    :BaseRepository<TagEntity>(dbContext, minioClient), ITagRepository
{
    public async Task<TagEntity?> GetByNameAsync(string name)
    {
        return await DbContext.Set<TagEntity>().FirstOrDefaultAsync(t => t.Name == name);
    }
    
}
