using System.ComponentModel.DataAnnotations;
using Seiun.Resources;
using Seiun.Utils;

namespace Seiun.Entities;

public class WordBookEntity : BaseEntity
{
    [MaxLength(Constants.WordBookName.MaxWordBookNameLength,
        ErrorMessage = ErrorMessages.ValidationError.OverWordBookNameLength)]
    public required string WordBookName { get; set; }

    public virtual ICollection<WordWordBookEntity> Words { get; set; } = [];
}