using Seiun.Entities;

namespace Seiun.Repositories;

public interface IPublicAnnouncementRepository : IBaseRepository<PublicAnnouncementEntity>
{
    Task<List<PublicAnnouncementEntity>?> GetRecentlyPublicAnnouncement();
}