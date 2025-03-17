using Seiun.Utils;
using System.ComponentModel.DataAnnotations;
using Seiun.Resources;

namespace Seiun.Entities;

public class AiArticleEntity : BaseEntity
{
	public required Guid UserId { get; set; }	
	
	[MaxLength(Constants.Article.MaxArticleLength, ErrorMessage = ErrorMessages.ValidationError.OverArticleLength)]
	public required string Article {get; set; }
	public required Guid SessionId { get; set; }
	
	[MaxLength(Constants.Article.MaxCoverUrlLength, ErrorMessage = ErrorMessages.ValidationError.OverCoverUrlLength)]
	public required string CoverUrl { get; set; }
	public required DateTime CreatedAt { get; set; }
}