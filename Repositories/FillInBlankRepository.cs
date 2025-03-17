using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Utils;
using Minio.DataModel.Args;
using Seiun.Entities;
using System.Net.Mime;

namespace Seiun.Repositories;

public class FillInBlankRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<FillInBlankEntity>(dbContext, minioClient), IFillInBlankRepository
{
    
}