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
/// 评论相关接口
/// </summary>
/// <param name="logger">日志</param>
/// <param name="repository">仓库服务</param>
[ApiController]
[Route("/api/comment")]
public class CommentController(ILogger<CommentController> logger, IRepositoryService repository)
    : ControllerBase
{
    /// <summary>
    /// 创建评论
    /// </summary>
    /// <param name="commentCreate">评论信息DTO</param>
    /// <returns>操作结果</returns>
    [HttpPost("create", Name = "CreateComment")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CommentCreate commentCreate)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }

        var article = await repository.ArticleRepository.GetByIdAsync(commentCreate.ArticleId);
        if (article == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Article.ArticleNotFound
            ));
        }

        var comment = new CommentEntity
        {
            UserId = (Guid)userId,
            Content = commentCreate.Content,
            PostId = commentCreate.ArticleId,
            LikeCount = 0,
            DislikeCount = 0,
            CreatedAt = DateTime.UtcNow
        };
        repository.CommentRepository.Create(comment);
        if (await repository.CommentRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.Comment.CreateSuccess));
        }

        logger.LogError("Comment creation failed. UserId: {UserId}, PostId: {PostId}",userId, commentCreate.ArticleId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Comment.CreateFailed
        ));
    }

    /// <summary>
    /// 删除评论
    /// </summary>
    /// <param name="id">评论Id</param>
    /// <returns>操作结果</returns>
    [HttpDelete("delete/{id:guid}", Name = "DeleteComment")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.ValidationError.CommentIdRequired
            ));
        }

        var comment = await repository.CommentRepository.GetByIdAsync(id);
        if (comment == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Comment.CommentNotFound
            ));
        }

        repository.CommentRepository.Delete(comment);
        if (await repository.CommentRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.Comment.DeleteSuccess));
        }

        logger.LogError("Comment delete failed. CommentID: {}", id);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Comment.CommentDeleteFailed
        ));
    }

    /// <summary>
    /// 获取评论详情
    /// </summary>
    /// <param name="commentId">评论Id</param>
    /// <returns>操作结果</returns>
    [HttpGet("detail", Name = "DetailComment")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(CommentDetailResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Detail(Guid commentId)
    {
        if (commentId == Guid.Empty)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.ValidationError.CommentIdRequired
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
        var commentInfo = new CommentInfo
        {
            CommentId = comment.Id,
            UserId  = comment.UserId,
            Content = comment.Content,
            PostId = comment.PostId,
            LikeCount = comment.LikeCount,
            DislikeCount = comment.DislikeCount,
            CreatedAt = comment.CreatedAt
        };
        return Ok(CommentDetailResp.Success(SuccessMessages.Controller.Comment.DetailSuccess, commentInfo));
    }

    /// <summary>
    /// 获取评论列表
    /// </summary>
    /// <param name="articleId">文章Id</param>
    /// <returns>操作结果</returns>
    [HttpGet("list", Name = "ListComment")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(CommentListResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromBody] Guid articleId)
    {
        if (articleId == Guid.Empty)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.ValidationError.CommentIdRequired
            ));
        }

        var article = await repository.ArticleRepository.GetByIdAsync(articleId);
        if (article == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Article.ArticleNotFound
            ));
        }

        var comments = (await repository.CommentRepository.GetListByPostIdAsync(articleId)).ToList();
        if (comments.Count == 0)
        {
            return Ok(CommentListResp.Success(
                ErrorMessages.Controller.Comment.CommentNotFound,
                []
            ));
        }

        var commentList = comments.Select(comment => new CommentInfo
        {
            CommentId = comment.Id,
            UserId = comment.UserId,
            Content = comment.Content,
            PostId = comment.PostId,
            LikeCount = comment.LikeCount,
            DislikeCount = comment.DislikeCount,
            CreatedAt = comment.CreatedAt
        }).ToList();

        return Ok(CommentListResp.Success(SuccessMessages.Controller.Comment.GetListSuccess, commentList));
    }

    /// <summary>
    /// 点赞评论
    /// </summary>
    /// <param name="commentId">评论Id</param>
    /// <returns>操作结果</returns>
    [HttpPost("like", Name = "LikeComment")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Like([FromBody] Guid commentId)
    {
        if (commentId == Guid.Empty)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.ValidationError.CommentIdRequired
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

        var comment = await repository.CommentRepository.GetByIdAsync(commentId);
        if (comment == null)
        {
            return StatusCode(StatusCodes.Status404NotFound, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Comment.CommentNotFound
            ));
        }

        var existingLike = await repository.CommentLikeRepository.GetCommentLikeAsync((Guid)userId, commentId);
        if (existingLike != null && existingLike.Action == ActionType.Like)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Comment.AlreadyLiked
            ));
        }

        if (existingLike != null && existingLike.Action == ActionType.Dislike)
        {
            repository.CommentLikeRepository.Delete(existingLike);
            comment.DislikeCount -= 1;
        }

        var likeRecord = new CommentLikeEntity
        {
            UserId = (Guid)userId,
            CommentId = commentId,
            Action = ActionType.Like,
            CreatedAt = DateTime.UtcNow
        };
        repository.CommentLikeRepository.Create(likeRecord);

        comment.LikeCount += 1;
        repository.CommentRepository.Update(comment);

        if (await repository.CommentRepository.SaveAsync() && !await repository.CommentLikeRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.Comment.GetLikeSuccess));
        }


        logger.LogError("Like operation failed. UserId: {UserId}, CommentId: {CommentId}", userId, commentId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Comment.GetLikeFailed
        ));
    }

    /// <summary>
    /// 取消点赞
    /// </summary>
    /// <param name="commentId">评论Id</param>
    /// <returns>操作结果</returns>
    [HttpPost("cancellike", Name = "CancelCommentLike")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelLike([FromBody] Guid commentId)
    {
        if (commentId == Guid.Empty)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.ValidationError.CommentIdRequired
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

        var comment = await repository.CommentRepository.GetByIdAsync(commentId);
        if (comment == null)
        {
            return StatusCode(StatusCodes.Status404NotFound, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Comment.CommentNotFound
            ));
        }

        var existingLike = await repository.CommentLikeRepository.GetCommentLikeAsync((Guid)userId, commentId);
        if (existingLike == null || existingLike.Action != ActionType.Like)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Comment.AlreadyCancelLiked
            ));
        }

        repository.CommentLikeRepository.Delete(existingLike);
        comment.LikeCount -= 1;
        repository.CommentRepository.Update(comment);
        
        if (await repository.CommentRepository.SaveAsync() && !await repository.CommentLikeRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.Comment.CancelLikeSuccess));
        }

        logger.LogError("Cancel Like operation failed. UserId: {UserId}, CommentId: {CommentId}", userId, commentId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Comment.CancelLikeFailed
        ));
        
    }

    /// <summary>
    /// 点赞
    /// </summary>
    /// <param name="commentId">评论Id</param>
    /// <returns>操作结果</returns>
    [HttpPost("dislike",Name = "DislikeComment")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Dislike([FromBody] Guid commentId)
    {
        if (commentId == Guid.Empty)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.ValidationError.CommentIdRequired
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

        var comment = await repository.CommentRepository.GetByIdAsync(commentId);
        if (comment == null)
        {
            return StatusCode(StatusCodes.Status404NotFound, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Comment.CommentNotFound
            ));
        }

        var existingDislike = await repository.CommentLikeRepository.GetCommentLikeAsync((Guid)userId, commentId);
        if (existingDislike != null && existingDislike.Action == ActionType.Dislike)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Comment.AlreadyDisliked
            ));
        }

        if (existingDislike != null && existingDislike.Action == ActionType.Like)
        {
            repository.CommentLikeRepository.Delete(existingDislike);
            comment.LikeCount -= 1;
        }

        var dislikeRecord = new CommentLikeEntity
        {
            UserId = (Guid)userId,
            CommentId = commentId,
            Action = ActionType.Dislike,
            CreatedAt = DateTime.UtcNow
        };
        repository.CommentLikeRepository.Create(dislikeRecord);
        comment.DislikeCount += 1;
        repository.CommentRepository.Update(comment);

        if (await repository.CommentRepository.SaveAsync() && !await repository.CommentLikeRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.Comment.GetDislikeSuccess));
        }

        logger.LogError("Dislike operation failed. UserId: {UserId}, CommentId: {CommentId}", userId, commentId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Comment.GetDislikeFailed
        ));
    }

    /// <summary></summary>
    /// 取消踩
    /// <param name="commentId">评论Id</param>
    /// <returns>操作结果</returns>
    [HttpPost("canceldislike",Name = "CancelCommentDislike")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")] 
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelDislike([FromBody] Guid commentId)
    {
        if (commentId == Guid.Empty)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.ValidationError.CommentIdRequired
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

        var comment = await repository.CommentRepository.GetByIdAsync(commentId);
        if (comment == null)
        {
            return StatusCode(StatusCodes.Status404NotFound, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Comment.CommentNotFound
            ));
        }
        var existingDislike = await repository.CommentLikeRepository.GetCommentLikeAsync((Guid)userId, commentId);
        if (existingDislike == null || existingDislike.Action != ActionType.Dislike)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status400BadRequest,
                ErrorMessages.Controller.Comment.AlreadyCancelDisliked
            ));
        }

        repository.CommentLikeRepository.Delete(existingDislike);
        comment.DislikeCount -= 1;
        repository.CommentRepository.Update(comment);

        if (await repository.CommentRepository.SaveAsync() && !await repository.CommentLikeRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.Comment.CancelDislikeSuccess));
        }

        logger.LogError("CancelDislike operation failed. UserId: {UserId}, CommentId: {CommentId}", userId, commentId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.Comment.CancelDislikeFailed
        ));
    }
}
