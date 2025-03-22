using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class FillInBlankWordRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<FillInBlankWordEntity>(dbContext, minioClient), IFillInBlankWordRepository
{
    public void BulkAdd(List<FillInBlankWordEntity> words)
    {
        DbContext.FillInBlankWords.AddRange(words);
    }

    public async Task<List<FillInBlankWordEntity>?> GetByQuestionIdAsync(Guid questionId)
    {
        return await DbContext.FillInBlankWords
            .Where(w => w.QuestionId == questionId)
            .ToListAsync();
    }
}