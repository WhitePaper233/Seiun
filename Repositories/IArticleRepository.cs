using Seiun.Entities;

namespace Seiun.Repositories;

public interface IArticleRepository : IBaseRepository<ArticleEntity>
{
    Task<List<Guid>?> GetArticleListAsync(int queryLength, DateTimeOffset? from);
    Task<List<Guid>?> GetArticleListByUserIdAsync(Guid userId);
    Task<string> UploadArticleImgAsync(Stream articleImgData);
    Task<bool> DeleteArticleImgAsync(List<string> articleImgName);
    Task<MemoryStream> GetArticleImgAsync(string fileNames);
    Task<List<ArticleEntity>> GetAllArticlesAsync(int index, int size, Guid? keyword);
    Task<int> GetTotalArticlesAsync(Guid? keyword);
}