using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class MistakeBookRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<MistakeBookEntity>(dbContext, minioClient), IMistakeBookRepository
{
}