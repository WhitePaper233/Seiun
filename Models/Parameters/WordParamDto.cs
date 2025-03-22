using System.ComponentModel.DataAnnotations;
using Seiun.Resources;

namespace Seiun.Models.Parameters;

public class WordResultDto
{
    [Required(ErrorMessage = ErrorMessages.ValidationError.WordIdRequired)]
    public required Guid WordId { get; set; }

    [Required(ErrorMessage = ErrorMessages.ValidationError.SessionIdRequired)]
    public required Guid SessionId { get; set; }
}

// 获取所有单词Dto
public class GetWordssByAdmin
{
    public required int Index { get; set; }
    public required int Size { get; set; }
    public Guid? Keyword { get; set; }
}