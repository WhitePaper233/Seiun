using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Utils;
using Minio.DataModel.Args;
using Seiun.Entities;
using System.Net.Mime;

namespace Seiun.Repositories;

public class ArticleRepository(SeiunDbContext dbContext, IMinioClient minioClient)
	: BaseRepository<ArticleEntity>(dbContext, minioClient), IArticleRepository
{
	public async Task<List<Guid>?> GetArticleListAsync(int queryLength, DateTime? from)
	{
		if(queryLength > 0)
		{
			queryLength = queryLength > 10 ? 10 : queryLength;
		}
		else
		{
			queryLength = 10;
		}

		if(!from.HasValue)
		{
			from = DateTime.Now;
		}

		var articleIds = await DbContext.Articles
			.Where(a => a.CreateTime <= from )
			.OrderByDescending(a => a.CreateTime)
			.Take(queryLength)
			.Select(a => a.Id)
			.ToListAsync();
		
		return articleIds;
	}

	public async Task<List<Guid>?> GetArticleListByUserIdAsync(Guid userId)
	{
		var articleIds = await DbContext.Articles
			.Where(a => a.CreatorId == userId)
			.OrderByDescending(a => a.IsPinned == true)
			.ThenByDescending(a => a.PinTime)
			.ThenByDescending(a => a.CreateTime)
			.Select(a => a.Id)
			.ToListAsync();

		return articleIds;
	}

	public async Task<string> UploadArticleImgAsync(Stream articleimgData, string BucketName)
	{
		var articleImgName = $"{Guid.NewGuid()}.webp";
		var putObjectArgs = new PutObjectArgs()
			.WithBucket(BucketName)
			.WithObject(articleImgName)
			.WithContentType(MediaTypeNames.Image.Webp)
			.WithStreamData(articleimgData)
			.WithObjectSize(articleimgData.Length);
		await MinioCl.PutObjectAsync(putObjectArgs);

		return articleImgName;
	}

	public async Task<bool> DeleteAticleImgAsync(List<string> articleImgNames, string BucketName)
	{
		foreach(var articleImgName in articleImgNames)
		{
			try
			{
				var removeObjectArgs = new RemoveObjectArgs()
					.WithBucket(BucketName)
					.WithObject(articleImgName);
				
				await MinioCl.RemoveObjectAsync(removeObjectArgs).ConfigureAwait(false);
			}
			catch
			{
				return false;
			}		
		}

		return true;
	}

	public async Task<MemoryStream> GetArticleImgAsync(string fileName, string BucketName)
	{
		var articleImgStream = new MemoryStream();
		var getObjectArgs = new GetObjectArgs()
			.WithBucket(BucketName)
			.WithObject(fileName)
			.WithCallbackStream(data => data.CopyTo(articleImgStream));
		await MinioCl.GetObjectAsync(getObjectArgs).ConfigureAwait(false);
		articleImgStream.Seek(0, SeekOrigin.Begin);  // 重置流位置

		return articleImgStream;
	}
}