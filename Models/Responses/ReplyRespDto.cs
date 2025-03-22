namespace Seiun.Models.Responses;

/// <summary>
/// Reply 信息
/// </summary>
public class ReplyInfo
{
    public required Guid ReplyId { get; set; }
    public required Guid CommentId { get; set; }
    public required Guid UserId { get; set; }
    public required string Content { get; set; }
    public Guid? ParentReplyId { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}

/// <summary>
/// 回复详情响应
/// </summary>
public sealed class ReplyDetailResp(int code, string message, ReplyInfo? replyInfo)
    : BaseRespWithData<ReplyInfo>(code, message, replyInfo)
{
    public static ReplyDetailResp Success(string message, ReplyInfo replyInfo)
    {
        return new ReplyDetailResp(200, message, replyInfo);
    }

    public static ReplyDetailResp Fail(int code, string message)
    {
        return new ReplyDetailResp(code, message, null);
    }
}

public sealed class ReplyListResp(int code, string message, List<ReplyInfo>? replyList)
    : BaseRespWithData<List<ReplyInfo>>(code, message, replyList)
{
    public static ReplyListResp Success(string message, List<ReplyInfo> replyList)
    {
        return new ReplyListResp(200, message, replyList);
    }

    public static ReplyListResp Fail(int code, string message)
    {
        return new ReplyListResp(code, message, null);
    }
}