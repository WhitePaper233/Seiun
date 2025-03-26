using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public class MistakeBookRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<MistakeBookEntity>(dbContext, minioClient), IMistakeBookRepository
{
    public async Task<List<Guid>?> GetByStatus(Guid userId)
    {
        return await DbContext.MistakeBook
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => m.WordId)
            .ToListAsync();
    }
}