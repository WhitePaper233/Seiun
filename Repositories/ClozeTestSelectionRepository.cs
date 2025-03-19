using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Utils;
using Minio.DataModel.Args;
using Seiun.Entities;
using System.Net.Mime;

namespace Seiun.Repositories;

public class ClozeTestSelectionRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<ClozeTestSelectionEntity>(dbContext, minioClient), IClozeTestSelectionRepository
{
    public void BulkAdd(List<ClozeTestSelectionEntity> selections)
    {
        DbContext.ClozeTestSelections.AddRange(selections);
    }

    public async Task<List<ClozeTestSelectionEntity>?> GetByQuestionIdAsync(Guid questionId)
    {
        return await DbContext.ClozeTestSelections
            .Where(c => c.QuestionId == questionId)
            .OrderBy(c => c.Key)
            .ToListAsync();
    }
}