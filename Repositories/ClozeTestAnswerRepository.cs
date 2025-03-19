using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Utils;
using Minio.DataModel.Args;
using Seiun.Entities;
using System.Net.Mime;

namespace Seiun.Repositories;

public class ClozeTestAnswerRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<ClozeTestAnswerEntity>(dbContext, minioClient), IClozeTestAnswerRepository
{
    public void BulkAdd(List<ClozeTestAnswerEntity> answers)
    {
        DbContext.ClozeTestAnswers.AddRange(answers);
    }

    public async Task<List<ClozeTestAnswerEntity>?> GetByQuestionIdAsync(Guid questionId)
    {
        return await DbContext.ClozeTestAnswers
            .Where(w => w.QuestionId == questionId)
            .OrderBy(w => w.Key)
            .ToListAsync();
    }
}