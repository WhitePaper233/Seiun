using Seiun.Entities;

namespace Seiun.Repositories;

public interface ICommentRepository : IBaseRepository<CommentEntity>
{
    Task<IEnumerable<CommentEntity>> GetListByPostIdAsync(Guid postId);
}

public interface ICommentLikeRepository : IBaseRepository<CommentLikeEntity>
{
    Task<CommentLikeEntity?> GetCommentLikeAsync(Guid userId, Guid commentId);
}

