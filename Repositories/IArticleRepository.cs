using Seiun.Entities;

namespace Seiun.Repositories;

public interface IArticleRepository : IBaseRepository<ArticleEntity>
{
    Task<List<Guid>?> GetArticleListAsync(int queryLength, DateTimeOffset? from);
    Task<List<Guid>?> GetArticleListByUserIdAsync(Guid userId);
    Task<string> UploadArticleImgAsync(Stream articleImgData, string bucketName);
    Task<bool> DeleteArticleImgAsync(List<string> articleImgName, string bucketName);
    Task<MemoryStream> GetArticleImgAsync(string fileNames, string bucketName);
    Task<List<ArticleEntity>> GetAllArticlesAsync(int index, int size, Guid? keyword);
    Task<int> GetTotalArticlesAsync(Guid? keyword);
}