using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Utils;
using Minio.DataModel.Args;
using Seiun.Entities;
using System.Net.Mime;

namespace Seiun.Repositories;

public class FillInBlankWordRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<FillInBlankWordEntity>(dbContext, minioClient), IFillInBlankWordRepository
{
    public void BulkAdd(List<FillInBlankWordEntity> words)
    {
        DbContext.FillInBlankWords.AddRange(words);
    }
}