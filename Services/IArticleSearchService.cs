using Seiun.Entities;

namespace Seiun.Services;

public interface IArticleSearchService
{
    Task<List<Guid>?> ArticleSearchAsync(string query, int page = 1, int pageSize = 10);
}