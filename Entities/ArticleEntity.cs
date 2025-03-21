using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
	// 置顶
	public required bool IsPinned { get; set; }
	// 置顶时间
	public DateTimeOffset? PinTime { get; set; }
	
	[ForeignKey(nameof(this.CreatorId))]
	public virtual UserEntity Creator { get; set; } = null!;

	[JsonIgnore]
	public virtual ICollection<ArticleLikeEntity> Likes { get; set; } = [];
	
	[JsonIgnore]	
	public virtual ICollection<CommentEntity> Comments { get; set; } = [];
}

public class ArticleLikeEntity: BaseEntity
{
	// 用户ID
	public required Guid UserId { get; set; }
	// 文章ID
	public required Guid LikedArticleId {get; set; }
	// 点赞时间
	public required DateTimeOffset LikedTime {get; set; }
	
	[ForeignKey(nameof(this.LikedArticleId))]
	public virtual ArticleEntity LikedArticle { get; set; } = null!;
}
