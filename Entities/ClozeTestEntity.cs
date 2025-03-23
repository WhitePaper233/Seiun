using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Seiun.Utils;
using Seiun.Resources;

namespace Seiun.Entities;

public class ClozeTestEntity : BaseEntity
{
    [Column(TypeName = "TEXT")] public required string ClozeDetail { get; set; }
}