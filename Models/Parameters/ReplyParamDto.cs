using System.ComponentModel.DataAnnotations;
using Seiun.Resources;

namespace Seiun.Models.Parameters;

public class ReplyCreate
{
    [Required(ErrorMessage = ErrorMessages.ValidationError.ContentRequired)]
    [MaxLength(500, ErrorMessage = ErrorMessages.ValidationError.ContentRequired)]
    public required string Content { get; set; }

    [Required(ErrorMessage = ErrorMessages.ValidationError.CommentIdRequired)]
    public required Guid CommentId { get; set; } 
    public Guid? ParentReplyId { get; set; }

}
