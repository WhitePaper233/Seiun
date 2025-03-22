using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seiun.Entities;
using Seiun.Models.Parameters;
using Seiun.Models.Responses;
using Seiun.Resources;
using Seiun.Services;
using Seiun.Utils;
using Seiun.Utils.Enums;

namespace Seiun.Controllers;

/// <summary>
/// 回复相关接口
/// </summary>
/// <param name="logger">日志</param>
/// <param name="repository">仓库服务</param>
[ApiController]
[Route("/api/reply")]
public class ReplyController(ILogger<ReplyController> logger, IRepositoryService repository)
    : ControllerBase
{
    /// <summary>
    /// 创建回复
    /// </summary>
    /// <param name="replyCreate">回复信息DTO</param>
    /// <returns>操作结果</returns>
    [HttpPost("create", Name = "CreateReply")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)}{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] ReplyCreate replyCreate)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }

        var comment = await repository.CommentRepository.GetByIdAsync(replyCreate.CommentId);
        if (comment == null)
        {
            return StatusCode(StatusCodes.Status404NotFound, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Comment.CommentNotFound
            ));
        }


        if (replyCreate.ParentReplyId.HasValue)
        {
            var parentReply = await repository.ReplyRepository.GetByIdAsync(replyCreate.ParentReplyId.Value);
            if (parentReply == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ResponseFactory.NewFailedBaseResponse(
                    StatusCodes.Status404NotFound,
                    ErrorMessages.Controller.Reply.ParentReplyNotFound
                ));
            }
        }

        var reply = new ReplyEntity
        {
            UserId = (Guid)userId,          
            Content = replyCreate.Content,  
            CommentId = replyCreate.CommentId, 
            ParentReplyId = replyCreate.ParentReplyId
            // CreatedAt = DateTime.UtcNow    
        };

        repository.ReplyRepository.Create(reply);
        if (await repository.ReplyRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.Reply.CreateSuccess));
        }

        logger.LogError("Create Reply failed for UserId: {UserId}, CommentId: {CommentId}", userId, replyCreate.CommentId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Reply.CreateFailed
        ));
    }

    /// <summary>
    /// 删除回复
    /// </summary>
    /// <param name="replyId">回复ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("delete/{replyId:guid}", Name = "DeleteReply")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)}{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid replyId)
    {
        if(replyId == Guid.Empty)
        {
            return BadRequest(ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Reply.ReplyIdRequired
            ));
        }
        var userId = User.GetUserId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }

        var reply = await repository.ReplyRepository.GetByIdAsync(replyId);
        if (reply == null)
        {
            return StatusCode(StatusCodes.Status404NotFound, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Reply.ReplyNotFound
            ));
        }

        repository.ReplyRepository.Delete(reply);
        if (await repository.ReplyRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.Reply.DeleteSuccess));
        }

        logger.LogError("Delete Reply failed for UserId: {UserId}, ReplyId: {ReplyId}", userId, replyId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Reply.DeleteFailed
        ));
    }

    /// <summary>
    /// 获取回复详情
    /// </summary>
    /// <param name="replyId">回复Id</param>
    /// <returns>操作结果</returns>
    [HttpGet("detail" , Name = "ReplyDetail")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)}{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(ReplyDetailResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Detail([FromQuery] Guid replyId)
    {
        if (replyId == Guid.Empty)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Reply.ReplyIdRequired
            ));
        }
        var reply = await repository.ReplyRepository.GetByIdAsync(replyId);
        if (reply == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Reply.ReplyNotFound
            ));
        }
        var replyInfo = new ReplyInfo
        {
            ReplyId = reply.Id,
            CommentId = reply.CommentId,
            UserId  = reply.UserId,
            Content = reply.Content,
            CreatedAt = reply.CreatedAt
        };
        return Ok(ReplyDetailResp.Success(SuccessMessages.Controller.Reply.DetailSuccess, replyInfo));
    }

    /// <summary>
    /// 获取回复列表
    /// </summary>
    /// <param name="commentId">评论Id</param>
    /// <returns>操作结果</returns>
    [HttpGet("list",Name = "ReplyList")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)}{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(ReplyListResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromQuery] Guid commentId)
    {
        if (commentId == Guid.Empty)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Reply.ReplyIdRequired
            ));
        }

        var comment = await repository.CommentRepository.GetByIdAsync(commentId);
        if (comment == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Comment.CommentNotFound
            ));
        }

        var replies = (await repository.ReplyRepository.GetListByCommentIdAsync(commentId)).ToList();
        if (replies.Count == 0)
        {
            return Ok(ReplyListResp.Success(
                ErrorMessages.Controller.Comment.CommentNotFound,
                []
            ));
        }

        var replyList = replies.Select(reply => new ReplyInfo
        {
            ReplyId = reply.Id,
            CommentId = reply.CommentId,
            UserId = reply.UserId,
            Content = reply.Content,
            ParentReplyId = reply.ParentReplyId,
            CreatedAt = reply.CreatedAt
        }).ToList();

        return Ok(ReplyListResp.Success(SuccessMessages.Controller.Reply.GetListSuccess, replyList));
    }
}