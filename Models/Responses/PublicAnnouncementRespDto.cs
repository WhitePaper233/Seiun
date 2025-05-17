using Seiun.Entities;
using Seiun.Resources;

namespace Seiun.Models.Responses;

public class PublicAnnouncementList
{
	public required List<PublicAnnouncementEntity> PublicAnnouncements { get; set; }
}

public sealed class PublicAnnouncementResp(int code, string message, PublicAnnouncementList? publicAnnouncementList)
	: BaseRespWithData<PublicAnnouncementList>(code, message, publicAnnouncementList)
{
	public static PublicAnnouncementResp Success(List<PublicAnnouncementEntity> publicAnnouncements) => new(StatusCodes.Status200OK,
	SuccessMessages.Controller.PublicAnnouncement.GetPublicAnnouncementsSuccess,
	new PublicAnnouncementList
	{
		PublicAnnouncements = publicAnnouncements
	});

	public static PublicAnnouncementResp Fail(int code, string message) => new(code, message, null);
}
