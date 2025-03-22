using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class ClozeTestRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<ClozeTestEntity>(dbContext, minioClient), IClozeTestRepository
{
    
}