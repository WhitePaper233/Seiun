using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class ReplyRepository(SeiunDbContext dbContext, IMinioClient minioClient)
	: BaseRepository<ReplyEntity>(dbContext, minioClient), IReplyRepository
{
	public async Task<IEnumerable<ReplyEntity>> GetListByCommentIdAsync(Guid commentId)
	{
		var replies = await DbContext.Set<ReplyEntity>()
			.Where(reply => reply.CommentId == commentId)
			.ToListAsync();

		return replies.Count == 0 ? [] : replies;
	}
}
