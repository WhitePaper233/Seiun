using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seiun.Models.Parameters;
using Seiun.Models.Responses;
using Seiun.Services;
using Seiun.Resources;
using Seiun.Utils;
using Seiun.Utils.Enums;
using Seiun.Entities;

namespace Seiun.Controllers;

/// <summary>
/// 公告相关接口
/// </summary>
/// <param name="logger">日志</param>
/// <param name="repository">仓储服务</param>
[ApiController, Route("/api/public-announcement")]
public class PublicAnnouncementController(ILogger<PublicAnnouncementController> logger, IRepositoryService repository)
	: ControllerBase
{
	/// <summary>
	/// 发布公告
	/// </summary>
	/// <param name="PublicAnnouncementPublish">公告信息</param>
	/// <returns>发布结果</returns>
	[HttpPost("publish", Name = "Publish")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> PublishPublicAnnouncement([FromBody] PublicAnnouncementPublish PublicAnnouncementPublish)
	{
		var userId = User.GetUserId();
		if(userId == null)
		{
			return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
				StatusCodes.Status403Forbidden,
				ErrorMessages.Controller.Any.InvalidJwtToken
			));
		}

		var publicAnnouncement = new PublicAnnouncementEntity
		{
			Title = PublicAnnouncementPublish.Title,
			Content = PublicAnnouncementPublish.Content,
			AdminId = userId.Value	
		};

		repository.PublicAnnouncementRepository.Create(publicAnnouncement);
		if(await repository.PublicAnnouncementRepository.SaveAsync())
		{
			return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.PublicAnnouncement.PublishSuccess));
		}

		logger.LogError("Admin {} publish failed", userId);
        return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.PublicAnnouncement.PublishFailed
        ));
	}

	/// <summary>
	/// 删除公告
	/// </summary>
	/// <param name="publicAnnouncementId">公告ID</param>
	/// <returns>删除结果</returns>
	[HttpDelete("delete/{publicAnnouncementId:Guid}", Name = "DeletePublicAnnouncement")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(BaseResp), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> DeletePublicAnnouncement(Guid publicAnnouncementId)
	{
		var userId = User.GetUserId();
		if(userId == null)
		{
			return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
				StatusCodes.Status403Forbidden,
				ErrorMessages.Controller.Any.InvalidJwtToken
			));
		}

		var publicAnnouncement = await repository.PublicAnnouncementRepository.GetByIdAsync(publicAnnouncementId);
		if(publicAnnouncement == null)
		{
			return NotFound(ResponseFactory.NewFailedBaseResponse(
				StatusCodes.Status404NotFound,
				ErrorMessages.Controller.PublicAnnouncement.AnnouncementNotFound
			));
		}

		if(publicAnnouncement.AdminId != userId)
		{
			return StatusCode(StatusCodes.Status403Forbidden, ResponseFactory.NewFailedBaseResponse(
				StatusCodes.Status403Forbidden,
				ErrorMessages.Controller.PublicAnnouncement.NotAuthorized
			));
		}

		repository.PublicAnnouncementRepository.Delete(publicAnnouncement);
		if(await repository.PublicAnnouncementRepository.SaveAsync())
		{
			return Ok(ResponseFactory.NewSuccessBaseResponse(SuccessMessages.Controller.PublicAnnouncement.DeleteSuccess));
		}

		logger.LogError("Admin {} delete publicannouncement {} failed", userId, publicAnnouncementId);
		return StatusCode(StatusCodes.Status500InternalServerError, ResponseFactory.NewFailedBaseResponse(
            StatusCodes.Status500InternalServerError,
            ErrorMessages.Controller.PublicAnnouncement.DeleteFailed
		));
	}

	/// <summary>
	/// 获取公告
	/// </summary>
	/// <returns>获取结果</returns>
	[HttpGet("get", Name = "GetPublicAnnouncement")]
	[ProducesResponseType(typeof(PublicAnnouncementResp), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(PublicAnnouncementResp), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetPublicAnnouncement()
	{
		var publicAnnouncements = await repository.PublicAnnouncementRepository.GetRecentlyPublicAnnouncement();
		if(publicAnnouncements == null)
		{
			return NotFound(PublicAnnouncementResp.Fail(
				StatusCodes.Status404NotFound,
				ErrorMessages.Controller.PublicAnnouncement.AnnouncementNotFound
			));
		}

		return Ok(PublicAnnouncementResp.Success(publicAnnouncements));
	}
}