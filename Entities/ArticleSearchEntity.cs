namespace Seiun.Entities;

// 用于搜索文章的实体
public class ArticleSearchEntity
{
    public required string Article { get; set; }

    public required string CreatorUserName { get; set; }

    public required string CreatorNickName { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required Guid ArticleId { get; set; }
}