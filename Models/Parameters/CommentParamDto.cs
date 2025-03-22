using System.ComponentModel.DataAnnotations;
using Seiun.Resources;

namespace Seiun.Models.Parameters;

public class CommentCreate
{
    [Required(ErrorMessage = ErrorMessages.ValidationError.ContentRequired)]
    [MaxLength(500, ErrorMessage = ErrorMessages.ValidationError.OverContentLength)]
    public required string Content { get; set; }

    [Required(ErrorMessage = ErrorMessages.ValidationError.ArticleIdRequired)]
    public required Guid ArticleId { get; set; }
}