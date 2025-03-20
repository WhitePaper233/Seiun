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
    IUserTagRepository UserTagRepository { get; }
    IWordSessionRepository SessionRepository { get; }
    IErrorWordRepository ErrorWordRepository { get; }
    IFinishedWordRepository FinishedWordRepository { get; }

    IUserCheckInRepository UserCheckInRepository { get; }
    IAIArticleRepository AiArticleRepository { get; }
    IFillInBlankRepository FillInBlankRepository { get; }
    IFillInBlankAnswerRepository FillInBlankAnswerRepository { get; }
    IFillInBlankWordRepository FillInBlankWordRepository { get; }
    IUserQuestionRepository UserQuestionRepository { get; }
    IClozeTestRepository ClozeTestRepository { get; }
    IClozeTestSelectionRepository ClozeTestSelectionRepository { get; }
    IClozeTestAnswerRepository ClozeTestAnswerRepository { get; }
}