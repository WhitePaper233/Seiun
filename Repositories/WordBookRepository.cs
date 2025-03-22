using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class WordBookRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<WordBookEntity>(dbContext, minioClient), IWordBookRepository
{
}