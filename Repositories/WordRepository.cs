using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;
using Seiun.Entities;
using Seiun.Utils;

namespace Seiun.Repositories;

public class WordRepository(SeiunDbContext dbContext, IMinioClient minioClient)
	: BaseRepository<WordEntity>(dbContext, minioClient), IWordRepository
{
	public async Task<List<WordEntity>> GetReviewingWordsByGuidsAsync(List<Guid> reviewingWordIds)
	{
		return await DbContext.Words
			.Where(w => reviewingWordIds.Contains(w.Id))
			.Include(w => w.WordDistractors)
			.ToListAsync();
	}

	public async Task<List<WordEntity>> GetAllWordsAsync(int index, int size, Guid? keyword)
	{
		var query = DbContext.Words
			.Include(w => w.WordDistractors)
			.Include(w => w.Books)
			.ThenInclude(b => b.Book)
			.AsQueryable();

		if (keyword.HasValue) query = query.Where(w => w.Id == keyword.Value);

		return await query
			.Skip((index - 1) * size)
			.Take(size)
			.ToListAsync();
	}

	public async Task<int> GetTotalWordsAsync(Guid? keyword)
	{
		var query = DbContext.Words.AsQueryable();
		if (keyword.HasValue) query = query.Where(w => w.Id == keyword.Value);
		return await query.CountAsync();
	}

	/// <summary>
	/// 获取单词助记图片
	/// </summary>
	/// <param name="fileName"></param>
	/// <returns>图片文件流</returns>
	public async Task<MemoryStream> GetWordMnemonicImage(string fileName)
	{
		var wordImgStream = new MemoryStream();
		var getObjectArgs = new GetObjectArgs()
			.WithBucket(Constants.BucketNames.WordMnemonicImage)
			.WithObject(fileName)
			.WithCallbackStream(data => data.CopyTo(wordImgStream));
		await MinioCl.GetObjectAsync(getObjectArgs).ConfigureAwait(false);
		wordImgStream.Seek(0, SeekOrigin.Begin);
		return wordImgStream;
	}

	/// <summary>
	/// 获取单词音频
	/// </summary>
	/// <param name="fileName">文件名</param>
	/// <returns>音频文件流</returns>
	public async Task<MemoryStream> GetWordAudio(string fileName)
	{
		var wordAudioStream = new MemoryStream();
		var getObjectArgs = new GetObjectArgs()
			.WithBucket(Constants.BucketNames.WordAudio)
			.WithObject(fileName)
			.WithCallbackStream(data => data.CopyTo(wordAudioStream));
		await MinioCl.GetObjectAsync(getObjectArgs).ConfigureAwait(false);
		wordAudioStream.Seek(0, SeekOrigin.Begin);
		return wordAudioStream;
	}

	public async Task<WordEntity> GetWordDetailByIdAsync(Guid wordId)
	{
		return await DbContext.Words
			.Where(w => w.Id == wordId)
			.Include(w => w.WordDistractors)
			.FirstAsync();
	}

	public async Task<List<WordEntity>> GetWordsByWordTextAsync(List<string> wordTexts)
	{
		return await DbContext.Words
			.Where(w => wordTexts.Contains(w.WordText))
			.ToListAsync();
	}
}
