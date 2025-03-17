using Nest;
using Seiun.Entities;

namespace Seiun.Services;
public class ArticleSearchService(IElasticClient elasticClient) : IArticleSearchService
{
    public async Task<List<Guid>?> ArticleSearchAsync(string query, int page = 1, int pageSize = 10)
    {
        // 构建搜索请求
        var response = await elasticClient.SearchAsync<ArticleSearchEntity>(s => s
            .Query(q => q
                .Bool(b => b
                    .Should(
                        sh => sh.Term(t => t
                            .Field(art => art.CreatorUserName)  // 精确匹配 CreatorUserName 字段
                            .Value(query)
                        ),
                        sh => sh.Term(t => t
                            .Field(art => art.ArticleId)  // 精确匹配 ArticleId 字段
                            .Value(query)
                        ),
                        // 多字段匹配，包含 Article 和 CreatorNickName（Text 字段）
                        sh => sh.MultiMatch(mm => mm
                            .Query(query)
                            .Fields(f => f
                                .Field(art => art.Article)      
                                .Field(art => art.CreatorNickName)
                            )
                            .Type(TextQueryType.BestFields) 
                        )
                    )
                    .MinimumShouldMatch(1) 
                )
            )
            .From((page - 1) * pageSize)  // 分页起始位置
            .Size(pageSize)               // 每页数量
            .Sort(ss => ss.Descending(art => art.CreateTime)) // 按发布时间倒序
        );

        List<Guid> articleIds = [];
		foreach(var article in response.Documents)
		{
			articleIds.Add(article.ArticleId);
		}
        return articleIds; 
    }
}