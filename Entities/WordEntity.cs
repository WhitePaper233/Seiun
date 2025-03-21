using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Seiun.Resources;
using Seiun.Utils;
using Seiun.Utils.Enums;

namespace Seiun.Entities;

public class WordEntity : BaseEntity
{
    [Required]
    [MaxLength(Constants.Word.MaxWordTextLength, ErrorMessage = ErrorMessages.ValidationError.OverWordTextLength)]
    public required string WordText { get; set; }

    [MaxLength(200)]
    public string? Pronunciation { get; set; }

    [Required]
    [MaxLength(Constants.Word.MaxWordDefinitionLength, ErrorMessage = ErrorMessages.ValidationError.OverWordDefinitionLength)]
    public required string Definition { get; set; }
    
    public virtual ICollection<WordDistractorEntity> WordDistractors { get; set; } = [];
    
    public virtual ICollection<WordBankWordBookEntity> Books { get; set; } = [];
}

public class WordDistractorEntity : BaseEntity
{
    public required Guid WordId { get; set; }
    
    public required Guid DistractorId { get; set; }
    
    [ForeignKey(nameof(this.WordId))]
    public virtual WordEntity Word { get; set; } = null!;
}

