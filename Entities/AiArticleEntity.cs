using Seiun.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Seiun.Resources;

namespace Seiun.Entities;

public class AiArticleEntity : BaseEntity
{
    public required Guid UserId { get; set; }

    [MaxLength(Constants.Article.MaxArticleTitleLength,
        ErrorMessage = ErrorMessages.ValidationError.OverArticleTitleMaxLength)]
    public required string Title { get; set; }

    [MaxLength(Constants.Article.MaxArticleDescriptionLength,
        ErrorMessage = ErrorMessages.ValidationError.OverArticleDescriptionMaxLength)]
    public required string Description { get; set; }

    [MaxLength(Constants.Article.MaxArticleLength,
        ErrorMessage = ErrorMessages.ValidationError.OverArticleContentMaxLength)]
    public required string Content { get; set; }

    [MaxLength(Constants.Article.MaxArticleTagLength,
        ErrorMessage = ErrorMessages.ValidationError.OverArticleTagMaxLength)]
    public required string Tag { get; set; }

    [MaxLength(Constants.Article.MaxImgFileNameLength,
        ErrorMessage = ErrorMessages.ValidationError.OverImgFileNameLength)]
    public required string CoverFileName { get; set; }

    public required Guid SessionId { get; set; }

    [ForeignKey(nameof(this.UserId))] public virtual UserEntity User { get; set; } = null!;
}