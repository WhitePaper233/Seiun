using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class WordDistractorRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<WordDistractorEntity>(dbContext, minioClient), IWordDistractorRepository
{
    
}