using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class FillInBlankRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<FillInBlankEntity>(dbContext, minioClient), IFillInBlankRepository
{
}