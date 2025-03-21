using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;
using Seiun.Entities;
using Seiun.Utils;

namespace Seiun.Repositories;

public class WordBankWordBookRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<WordBankWordBookEntity>(dbContext, minioClient), IWordBankWordBookRepository
{
    public async Task<List<WordEntity>?> GetWordBookByTagAsync(Guid bookId, int dailyPlan, Guid userId)
    {
        var finishedWordIds = await DbContext.FinishedWords
            .Where(a => a.UserId == userId)
            .Select(f => f.WordId) 
            .ToListAsync();
        
        return await DbContext.WordBankWordBooks
            .Where(w => w.BookId == bookId)
            .Where(w => !finishedWordIds.Contains(w.Id))
            .Take(dailyPlan)
            .Select(w => w.Word)
            .ToListAsync();
    }
}