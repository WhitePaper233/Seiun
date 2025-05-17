using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Seiun.Entities;

public class WordSessionEntity : BaseEntity
{
	// 主键就是SessionId
	public required Guid UserId { get; set; }

	public required int ReviewingCount { get; set; }

	public required int StudyingCount { get; set; }

	public required List<Guid>? ReviewingWords { get; set; }

	public required List<Guid>? StudyingWords { get; set; }

	[ForeignKey(nameof(this.UserId))] public virtual UserEntity User { get; set; } = null!;

	[JsonIgnore] public virtual ICollection<ChallengeEntity> Challenges { get; set; } = [];
}
