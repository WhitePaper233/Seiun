namespace Seiun.Entities;

public class WordWordBookEntity : BaseEntity
{
	public required Guid WordId { get; set; }
	public required WordEntity Word { get; set; }
	public required Guid BookId { get; set; }
	public required WordBookEntity Book { get; set; }
}
