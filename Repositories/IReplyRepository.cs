using Seiun.Entities;

namespace Seiun.Repositories;

public interface IReplyRepository : IBaseRepository<ReplyEntity>
{
	Task<IEnumerable<ReplyEntity>> GetListByCommentIdAsync(Guid commentId);
}
