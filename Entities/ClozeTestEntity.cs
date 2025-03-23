using System.ComponentModel.DataAnnotations.Schema;


namespace Seiun.Entities;

public class ClozeTestEntity : BaseEntity
{
    [Column(TypeName = "TEXT")] public required string ClozeTestJson { get; set; }
}