using System.ComponentModel.DataAnnotations.Schema;
using Seiun.Utils.Enums;

namespace Seiun.Entities;

public class MistakeBookEntity : BaseEntity
{
	public required Guid UserId { get; set; }
	public required Guid WordId { get; set; }
	[ForeignKey(nameof(this.UserId))] public virtual UserEntity User { get; set; } = null!;
}
