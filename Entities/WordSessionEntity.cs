

using System.ComponentModel.DataAnnotations.Schema;

namespace Seiun.Entities;
public class WordSessionEntity : BaseEntity
{
    // 主键就是SessionId
    public required Guid UserId { get; set; }
    public required DateTimeOffset WordSessionAt { get; set; }
    
    [ForeignKey(nameof(this.UserId))]
    public virtual UserEntity User { get; set; } = null!;
}
