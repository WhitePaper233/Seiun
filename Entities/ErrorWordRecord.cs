using System.ComponentModel.DataAnnotations.Schema;

namespace Seiun.Entities;
public class ErrorWordRecordEntity : BaseEntity
{
    public required Guid UserId { get; set; }
    public required Guid WordId { get; set; }
    public required Guid SessionId { get; set; }
    
    [ForeignKey(nameof(this.UserId))]
    public virtual UserEntity User { get; set; } = null!;
}
