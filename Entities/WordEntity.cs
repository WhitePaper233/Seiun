using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Seiun.Resources;
using Seiun.Utils;

namespace Seiun.Entities;

public class WordEntity : BaseEntity
{
    [MaxLength(Constants.Word.MaxWordTextLength, ErrorMessage = ErrorMessages.ValidationError.OverWordTextLength)]
    public required string WordText { get; set; }

    [MaxLength(Constants.Word.MaxWordPronunciationLength,
        ErrorMessage = ErrorMessages.ValidationError.OverPronunciationLength)]
    public required string Pronunciation { get; set; }

    [MaxLength(Constants.Word.MaxWordDefinitionLength,
        ErrorMessage = ErrorMessages.ValidationError.OverWordDefinitionLength)]
    public required string Definition { get; set; }
    
    [MaxLength(Constants.Word.MaxWordPrimaryDefinitionLength, 
        ErrorMessage = ErrorMessages.ValidationError.OverWordPrimaryDefinitionLength)]
    public required string PrimaryDefinition  { get; set; }

    [MaxLength(Constants.Word.MaxWordExampleSentenceLength,
        ErrorMessage = ErrorMessages.ValidationError.OverWordExampleSentenceLength)]
    public required string ExampleSentence { get; set; }

    public virtual ICollection<WordDistractorEntity> WordDistractors { get; set; } = [];

    public virtual ICollection<WordWordBookEntity> Books { get; set; } = [];
}

public class WordDistractorEntity : BaseEntity
{
    public required Guid WordId { get; set; }

    public required Guid DistractorId { get; set; }

    [ForeignKey(nameof(this.WordId))] public virtual WordEntity Word { get; set; } = null!;
}