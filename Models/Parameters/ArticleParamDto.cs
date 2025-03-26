using System.ComponentModel.DataAnnotations;
using Seiun.Resources;

namespace Seiun.Models.Parameters;

public class ArticleCreate
{
    // 标题
    [Required(ErrorMessage = ErrorMessages.ValidationError.ArticleTitleRequired)]
    public required string Title { get; set; }

    // 简介
    public string? Description { get; set; }

    // 文章内容
    [Required(ErrorMessage = ErrorMessages.ValidationError.ArticleRequired)]
    public required string Article { get; set; }

    // Vocabulary
    public required string? Vocabulary { get; set; }

    // 图片
    public List<string>? ImageNames { get; set; }

    // 封面
    public string? CoverFileName { get; set; }
}

public class GetArticlesByAdmin
{
    public required int Index { get; set; }
    public required int Size { get; set; }
    public Guid? Keyword { get; set; } = null;
}