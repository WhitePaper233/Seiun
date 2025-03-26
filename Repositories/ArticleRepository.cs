using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;
using Seiun.Entities;
using System.Net.Mime;
using Seiun.Utils;

namespace Seiun.Repositories;

public class ArticleRepository(SeiunDbContext dbContext, IMinioClient minioClient)
    : BaseRepository<ArticleEntity>(dbContext, minioClient), IArticleRepository
{
    public async Task<List<Guid>?> GetArticleListAsync(int queryLength, DateTimeOffset? from)
    {
        if (queryLength > 0)
            queryLength = queryLength > 10 ? 10 : queryLength;
        else
            queryLength = 10;

        from ??= DateTimeOffset.UtcNow;

        return await DbContext.Articles
            .Where(a => a.CreatedAt <= from)
            .OrderByDescending(a => a.CreatedAt)
            .Take(queryLength)
            .Select(a => a.Id)
            .ToListAsync();
    }

    public async Task<List<Guid>?> GetArticleListByUserIdAsync(Guid userId)
    {
        return await DbContext.Articles
            .Where(a => a.CreatorId == userId)
            .OrderByDescending(a => a.IsPinned == true)
            .ThenByDescending(a => a.PinTime)
            .ThenByDescending(a => a.CreatedAt)
            .Select(a => a.Id)
            .ToListAsync();
    }

    public async Task<string> UploadArticleImgAsync(Stream articleimgData)
    {
        var articleImgName = $"{Guid.NewGuid()}.webp";
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(Constants.BucketNames.ArticleImages)
            .WithObject(articleImgName)
            .WithContentType(MediaTypeNames.Image.Webp)
            .WithStreamData(articleimgData)
            .WithObjectSize(articleimgData.Length);
        await MinioCl.PutObjectAsync(putObjectArgs);

        return articleImgName;
    }

    public async Task<bool> DeleteArticleImgAsync(List<string> articleImgNames)
    {
        foreach (var articleImgName in articleImgNames)
            try
            {
                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(Constants.BucketNames.ArticleImages)
                    .WithObject(articleImgName);

                await MinioCl.RemoveObjectAsync(removeObjectArgs).ConfigureAwait(false);
            }
            catch
            {
                return false;
            }

        return true;
    }

    public async Task<MemoryStream> GetArticleImgAsync(string fileName)
    {
        var articleImgStream = new MemoryStream();
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(Constants.BucketNames.ArticleImages)
            .WithObject(fileName)
            .WithCallbackStream(data => data.CopyTo(articleImgStream));
        await MinioCl.GetObjectAsync(getObjectArgs).ConfigureAwait(false);
        articleImgStream.Seek(0, SeekOrigin.Begin); // 重置流位置

        return articleImgStream;
    }
}