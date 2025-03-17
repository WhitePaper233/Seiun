using System.ComponentModel.DataAnnotations;
using Seiun.Resources;

namespace Seiun.Models.Parameters;

public class ArticleCreate
{
	// 文章markdown文本
	[Required(ErrorMessage = ErrorMessages.ValidationError.ArticleRequired)]
	public required string Article { get; set; }

	// 图片
	public List<string>? ImageNames { get; set; }

	// 封面
	public string? CoverFileName { get; set; }
}