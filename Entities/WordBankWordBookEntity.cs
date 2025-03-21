using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Seiun.Resources;
using Seiun.Utils;
using Seiun.Utils.Enums;

namespace Seiun.Entities;

public class WordBankWordBookEntity : BaseEntity
{
    public required Guid WordId { get; set; }
    public required WordEntity Word { get; set; }
    public required Guid BookId { get; set; }
    public required WordBookEntity Book { get; set; }
}