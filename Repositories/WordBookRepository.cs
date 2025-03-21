using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;
using Seiun.Entities;
using Seiun.Utils;

namespace Seiun.Repositories;

public class WordBookRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<WordBookEntity>(dbContext, minioClient), IWordBookRepository
{

}