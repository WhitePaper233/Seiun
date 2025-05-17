using System.ComponentModel.DataAnnotations;
using Seiun.Resources;

namespace Seiun.Models.Parameters;

public class PublicAnnouncementPublish
{
	// 标题
	[Required(ErrorMessage = ErrorMessages.ValidationError.PublicAnnouncementTitleRequired)]
	public required string Title { get; set; }

	// 内容
	[Required(ErrorMessage = ErrorMessages.ValidationError.PublicAnnouncementContentRequired)]
	public required string Content { get; set; }
}
