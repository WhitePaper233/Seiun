namespace Seiun.Models.Responses;



/// <summary>
/// Comment 信息
/// </summary>
public class CommentInfo
{
    public required Guid CommentId { get; set; }
    public required Guid UserId { get; set; }
    public required string Content { get; set; }
    public required Guid PostId { get; set; }
    public required int LikeCount { get; set; }
    public required int DislikeCount { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// 评论详情响应
/// </summary>
public sealed class CommentDetailResp(int code, string message, CommentInfo? commentInfo)
    : BaseRespWithData<CommentInfo>(code, message, commentInfo)
{
    public static CommentDetailResp Success(string message, CommentInfo commentInfo)
    {
        return new CommentDetailResp(200, message, commentInfo);
    }

    public static CommentDetailResp Fail(int code, string message)
    {
        return new CommentDetailResp(code, message, null);
    }
}

public sealed class CommentListResp(int code, string message, List<CommentInfo>? commentList)
    : BaseRespWithData<List<CommentInfo>>(code, message, commentList)
{
    public static CommentListResp Success(string message, List<CommentInfo> commentList)
    {
        return new CommentListResp(200, message, commentList);
    }

    public static CommentListResp Fail(int code, string message)
    {
        return new CommentListResp(code, message, null);
    }
}



