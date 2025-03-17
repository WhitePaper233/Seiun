using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seiun.Entities;
using Seiun.Models.Parameters;
using Seiun.Models.Responses;
using Seiun.Resources;
using Seiun.Services;
using Seiun.Utils;
namespace Seiun.Controllers;


[ApiController, Route("/api/tag/user")]
public class TagController(ILogger<UserController> logger, IRepositoryService repository)
    : ControllerBase
{
    
    /// <summary>
    /// 选择词库
    /// </summary>
    /// /// <param name="userSelectTag">用户选择的词库</param>
    /// <returns>操作结果</returns>
    [HttpPost("add", Name = "AddUserTag")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(UserProfileResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddUserTag([FromBody] UserSelectTag userSelectTag)
    {   
        var userId = User.GetUserId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }

        var user = await repository.UserRepository.GetByIdAsync(userId.Value);
        if (user == null)
        {
            return NotFound(UserProfileResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.User.UserNotFound
            ));
        }

        var tag = await repository.TagRepository.GetByIdAsync(userSelectTag.TagId);
        if (tag == null)
        {
            return NotFound(UserProfileResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.Tag.TagNotFound
            ));
        }
        var totalWords = tag.Words.Count;

        var remainingDays = totalWords / userSelectTag.DailyPlan;
        var userTag = new UserTagEntity{
            UserId = userId.Value,
            TagId = userSelectTag.TagId,
            DailyPlan = userSelectTag.DailyPlan,
            TotalDays = userSelectTag.TotalDays,
            RemainingDays = remainingDays,
            LearnedCount = 0,
            SettingAt = DateTime.Now,
            User = user,
            Tag = tag
        };
        repository.UserTagRepository.Create(userTag);
        if(await repository.UserTagRepository.SaveAsync())
        {
            return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.UserTag.CreateSuccess));
        }
        logger.LogError("Failed to create user tag");
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.UserTag.CreateFailed
        ));
    }

    
    /// <summary>
    /// 获取未选择的词库
    /// </summary>
    /// <returns>未选择的词库列表</returns>
    [HttpGet("unselected-tags", Name = "GetUnselectedTags")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(List<UserUnSelectTagRespDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUnselectedTags()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }
        var user = await repository.UserRepository.GetByIdAsync(userId.Value);
        if (user == null)
        {
            return NotFound(UserProfileResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.User.UserNotFound
            ));
        }

        var unselectedTags = await repository.UserTagRepository.GetUnselectedTagsAsync(userId.Value);
        var unselectedTagDtos = unselectedTags.Select(tag => new UserUnSelectTagRespDto
        {
            TagName = tag.Name,
            WordCount = tag.Words.Count
        }).ToList();

        return Ok(UserUnSelectTagListResp.Success(SuccessMessages.Controller.Comment.GetListSuccess, unselectedTagDtos));
    }


    /// <summary>
    /// 获取已选择的词库
    /// </summary>
    /// <returns>已选择的词库列表</returns>
    [HttpGet("selected-tags", Name = "GetSelectedTags")]
    [ProducesResponseType(typeof(List<UserUnSelectTagRespDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(UserProfileResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSelectedTags()
    {   
        var userId = User.GetUserId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }
        var user = await repository.UserRepository.GetByIdAsync(userId.Value);
        if (user == null)
        {
            return NotFound(UserProfileResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.User.UserNotFound
            ));
        }
        var selectedTags = await repository.UserTagRepository.GetSelectedTagsAsync(userId.Value);
        var selectedTagDtos = selectedTags.Select(tag => new UserSelectedTagRespDto
        {
            TagName = tag.Tag.Name,
            TotalCount = tag.Tag.Words.Count,
            LearnedCount = tag.LearnedCount,
        }).ToList();
        return Ok(UserSelectedTagListResp.Success(SuccessMessages.Controller.Comment.GetListSuccess, selectedTagDtos));
    }

    /// <summary>
    /// 取消用户词库
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>操作结果</returns>
    [HttpDelete("cancel", Name = "CancelUserTag")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(UserProfileResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> CancelUserTag([FromQuery] Guid tagId)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }
        var user = await repository.UserRepository.GetByIdAsync(userId.Value);
        if (user == null)
        {
            return NotFound(UserProfileResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.User.UserNotFound
            ));
        }
        await repository.UserTagRepository.CancelTagAsync(userId.Value, tagId);
        if(await repository.UserTagRepository.SaveAsync())
        {
           return Ok(SuccessMessages.Controller.UserTag.DeleteSuccess);
        }
        logger.LogError("Failed to delete user tag {userId} {tagId}", userId, tagId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.UserTag.DeleteFailed
        ));
    }
    

    /// <summary>
    /// 获取用户正在学习的词库详情
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>正在学习的词库详情</returns>
    [HttpDelete("detail", Name = "UserTagDetial")]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(UserProfileResp), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetUserStudyingTagDetail()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
                StatusCodes.Status403Forbidden,
                ErrorMessages.Controller.Any.InvalidJwtToken
            ));
        }

        // TODO: 应该在用户注册时，自动添加一个默认的词库
        var selectedTag = await repository.UserTagRepository.GetStudyingTagByUserIdAsync(userId.Value);
        if (selectedTag == null)
        {
            return NotFound(UserProfileResp.Fail(
                StatusCodes.Status404NotFound,
                ErrorMessages.Controller.User.UserNotFound
            ));  
        }
        var selectedTagDetail = new UserSelectedTagRespDto
        {
            TagName = selectedTag.Tag.Name,
            LearnedCount = selectedTag.LearnedCount,
            TotalCount = selectedTag.Tag.Words.Count
        };
        return Ok(SeletedTagDetailResp.Success(SuccessMessages.Controller.UserTag.GetDetailSuccess, selectedTagDetail));
    }
}