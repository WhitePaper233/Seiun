using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Seiun.Resources;
using Seiun.Utils.Enums;

namespace Seiun.Entities;

public class CommentEntity : BaseEntity
{
    public required Guid UserId { get; set; }

    [MaxLength(500, ErrorMessage = ErrorMessages.ValidationError.OverContentLength)]
    public required string Content { get; set; }

    public required Guid PostId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.ValidationError.InvalidLikeCount)]
    public int LikeCount { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = ErrorMessages.ValidationError.InvalidDisLikeCount)]
    public int DislikeCount { get; set; }

    [JsonIgnore] public virtual ICollection<CommentLikeEntity> CommentLikes { get; set; } = [];

    [ForeignKey(nameof(this.PostId))] public virtual ArticleEntity Article { get; set; } = null!;

    [JsonIgnore] public virtual ICollection<ReplyEntity> Replies { get; set; } = [];
}

public class CommentLikeEntity : BaseEntity
{
    public required Guid UserId { get; set; }

    public required Guid CommentId { get; set; }

    public ActionType Action { get; set; }

    [ForeignKey(nameof(this.UserId))] public virtual CommentEntity Comment { get; set; } = null!;
}