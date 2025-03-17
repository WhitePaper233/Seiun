namespace Seiun.Entities;

public class ArticleEntity: BaseEntity
{
	// 文章
	public required string Article { get; set; }
	// 图片
	public List<string>? ImageFileNames { get; set; }
	// 封面
	public string? CoverFileName { get; set; }
	// 发布者ID
	public required Guid CreatorId { get; set; }
	// 发布时间
	public required DateTime CreateTime { get; set; }
	// 置顶
	public required bool IsPinned { get; set; }
	// 置顶时间
	public DateTime? PinTime { get; set; }
}

public class ArticleLikeEntity: BaseEntity
{
	// 用户ID
	public required Guid UserId { get; set; }
	// 文章ID
	public required Guid LikedArticleId {get; set; }
	// 点赞时间
	public required DateTime LikedTime {get; set; }
}
