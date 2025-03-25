using System.ComponentModel.DataAnnotations.Schema;

namespace Seiun.Entities;

public class MistakeBookEntity : BaseEntity
{
    public required Guid UserId { get; set; }
    public required Guid SessionId { get; set; }
    public required Guid WordId { get; set; }
    public required Guid SelectedWordId { get; set; }
    public required bool FinishedStatus { get; set; }

    [ForeignKey(nameof(this.UserId))] public virtual UserEntity User { get; set; } = null!;
}