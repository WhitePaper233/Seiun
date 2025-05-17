using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Seiun.Resources;

namespace Seiun.Entities;

public class ReplyEntity : BaseEntity
{
	[MaxLength(500, ErrorMessage = ErrorMessages.ValidationError.OverContentLength)]
	public required string Content { get; set; }

	[Required(ErrorMessage = ErrorMessages.ValidationError.UserIdRequired)]
	public required Guid UserId { get; set; }

	[Required(ErrorMessage = ErrorMessages.ValidationError.CommentIdRequired)]
	public required Guid CommentId { get; set; }

	public Guid? ParentReplyId { get; set; }


	[ForeignKey(nameof(this.CommentId))] public virtual CommentEntity Comment { get; set; } = null!;
}
