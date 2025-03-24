using Seiun.Repositories;

namespace Seiun.Services;

public interface IRepositoryService
{
    IUserRepository UserRepository { get; }
    IArticleRepository ArticleRepository { get; }
    IArticleLikeRepository ArticleLikeRepository { get; }
    IPublicAnnouncementRepository PublicAnnouncementRepository { get; }
    ICommentRepository CommentRepository { get; }
    ICommentLikeRepository CommentLikeRepository { get; }
    IReplyRepository ReplyRepository { get; }
    IWordRepository WordRepository { get; }
    IUserPlansRepository UserPlansRepository { get; }
    IWordSessionRepository SessionRepository { get; }
    IWrongWordRepository WrongWordRepository { get; }
    IFinishedWordRepository FinishedWordRepository { get; }

    IUserCheckInRepository UserCheckInRepository { get; }
    IAiArticleRepository AiArticleRepository { get; }
    IChallengeRepository ChallengeRepository { get; }
    IWordBookRepository WordBookRepository { get; }
    IWordWordBookRepository WordWordBookRepository { get; }
}