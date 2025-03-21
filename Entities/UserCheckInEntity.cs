using System.ComponentModel.DataAnnotations.Schema;

namespace Seiun.Entities;

public class UserCheckInEntity : BaseEntity
{
    public required Guid UserId { get; set; }
    
    [ForeignKey(nameof(this.UserId))]
    public virtual UserEntity User { get; set; } = null!;
}
