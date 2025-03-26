using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;
using Seiun.Utils.Enums;

namespace Seiun.Repositories;

public class MistakeBookRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<MistakeBookEntity>(dbContext, minioClient), IMistakeBookRepository
{
    public async Task<List<Guid>?> GetByStatus(MistakeStatus mistakeStatus, Guid userId)
    {
        if (mistakeStatus == MistakeStatus.All)
            return await DbContext.MistakeBook
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => m.Id)
                .ToListAsync();


        return await DbContext.MistakeBook
            .Where(m => m.UserId == userId && m.Status == mistakeStatus)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => m.Id)
            .ToListAsync();
    }
}