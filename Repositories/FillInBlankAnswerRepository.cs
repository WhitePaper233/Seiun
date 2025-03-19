using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Utils;
using Minio.DataModel.Args;
using Seiun.Entities;
using System.Net.Mime;

namespace Seiun.Repositories;

public class FillInBlankAnswerRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<FillInBlankAnswerEntity>(dbContext, minioClient), IFillInBlankAnswerRepository
{
    public void BulkAdd(List<FillInBlankAnswerEntity> answers)
    { 
        DbContext.FillInBlankAnswers.AddRange(answers);
    }

    public async Task<List<FillInBlankAnswerEntity>?> GetByQuestionIdAsync(Guid questionId)
    {
        return await DbContext.FillInBlankAnswers
            .Where(a => a.QuestionId == questionId)
            .OrderBy(a => a.Key)
            .ToListAsync();
    }
}