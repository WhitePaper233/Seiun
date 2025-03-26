using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Seiun.Resources;
using Seiun.Utils;
using System.ComponentModel.DataAnnotations;

namespace Seiun.Entities;

public class ArticleEntity : BaseEntity
{
    // 标题
    [MaxLength(Constants.Article.MaxArticleTitleLength,
        ErrorMessage = ErrorMessages.ValidationError.OverArticleTitleMaxLength)]
    public required string Title { get; set; }

    // 简介
    [MaxLength(Constants.Article.MaxArticleDescriptionLength,
        ErrorMessage = ErrorMessages.ValidationError.OverArticleDescriptionMaxLength)]
    public string? Description { get; set; }

    // 文章
    [MaxLength(Constants.Article.MaxArticleLength, ErrorMessage = ErrorMessages.ValidationError.OverArticleMaxLength)]
    public required string Article { get; set; }

    // key vocabulary
    [MaxLength(Constants.Article.MaxArticleVocabularyLength,
        ErrorMessage = ErrorMessages.ValidationError.OverArticleVocabularyMaxLength)]
    public string? Vocabulary { get; set; }

    // 图片
    public List<string>? ImageFileNames { get; set; }

    // 封面
    [MaxLength(Constants.Article.MaxImgUrlLength, ErrorMessage = ErrorMessages.ValidationError.OverImgUrlLength)]
    public string? CoverFileName { get; set; }

    // 发布者ID
    public required Guid CreatorId { get; set; }

    // 置顶
    public required bool IsPinned { get; set; }

    // 置顶时间
    public DateTimeOffset? PinTime { get; set; }

    [ForeignKey(nameof(this.CreatorId))] public virtual UserEntity Creator { get; set; } = null!;

    [JsonIgnore] public virtual ICollection<ArticleLikeEntity> Likes { get; set; } = [];

    [JsonIgnore] public virtual ICollection<CommentEntity> Comments { get; set; } = [];
}

public class ArticleLikeEntity : BaseEntity
{
    // 用户ID
    public required Guid UserId { get; set; }

    // 文章ID
    public required Guid LikedArticleId { get; set; }

    // 点赞时间
    public required DateTimeOffset LikedTime { get; set; }

    [ForeignKey(nameof(this.LikedArticleId))]
    public virtual ArticleEntity LikedArticle { get; set; } = null!;
}