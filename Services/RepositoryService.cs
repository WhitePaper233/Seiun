using System.Runtime.Intrinsics.X86;
using Minio;
using Seiun.Entities;
using Seiun.Repositories;

namespace Seiun.Services;

public class RepositoryService(SeiunDbContext seiunDbContext, IMinioClient minioClient) : IRepositoryService
{
    private readonly Lazy<IUserRepository> _userRepository = new(() => new UserRepository(seiunDbContext, minioClient));
    public IUserRepository UserRepository => _userRepository.Value;

    private readonly Lazy<IArticleRepository> _articleRepository =
        new(() => new ArticleRepository(seiunDbContext, minioClient));

    public IArticleRepository ArticleRepository => _articleRepository.Value;

    private readonly Lazy<IArticleLikeRepository> _ArticleLikeRepository =
        new(() => new ArticleLikeRepository(seiunDbContext, minioClient));

    public IArticleLikeRepository ArticleLikeRepository => _ArticleLikeRepository.Value;

    private readonly Lazy<IPublicAnnouncementRepository> _publicAnnouncementRepository =
        new(() => new PublicAnnouncementRepository(seiunDbContext, minioClient));

    public IPublicAnnouncementRepository PublicAnnouncementRepository => _publicAnnouncementRepository.Value;

    private readonly Lazy<ICommentRepository> _commentRepository =
        new(() => new CommentRepository(seiunDbContext, minioClient));

    public ICommentRepository CommentRepository => _commentRepository.Value;

    private readonly Lazy<ICommentLikeRepository> _commentLikeRepository =
        new(() => new CommentLikeRepository(seiunDbContext, minioClient));

    public ICommentLikeRepository CommentLikeRepository => _commentLikeRepository.Value;

    private readonly Lazy<IReplyRepository> _replyRepository =
        new(() => new ReplyRepository(seiunDbContext, minioClient));

    public IReplyRepository ReplyRepository => _replyRepository.Value;

    private readonly Lazy<IWordRepository> _wordRepository = new(() => new WordRepository(seiunDbContext, minioClient));
    public IWordRepository WordRepository => _wordRepository.Value;

    private readonly Lazy<IUserPlansRepository> _userPlansRepository =
        new(() => new UserPlansRepository(seiunDbContext, minioClient));

    public IUserPlansRepository UserPlansRepository => _userPlansRepository.Value;

    private readonly Lazy<IWordSessionRepository> _sessionRepository =
        new(() => new WordSessionRepository(seiunDbContext, minioClient));

    public IWordSessionRepository SessionRepository => _sessionRepository.Value;

    private readonly Lazy<IErrorWordRepository> _errorWordRepository =
        new(() => new ErrorWordRepository(seiunDbContext, minioClient));

    public IErrorWordRepository ErrorWordRepository => _errorWordRepository.Value;

    private readonly Lazy<IFinishedWordRepository> _finishedWordRepository =
        new(() => new FinishedWordRepository(seiunDbContext, minioClient));

    public IFinishedWordRepository FinishedWordRepository => _finishedWordRepository.Value;

    private readonly Lazy<IAiArticleRepository> _aiArticleRepository =
        new(() => new AiArticleRepository(seiunDbContext, minioClient));

    public IAiArticleRepository AiArticleRepository => _aiArticleRepository.Value;

    private readonly Lazy<IUserCheckInRepository> _userCheckInRepository =
        new(() => new UserCheckInRepository(seiunDbContext, minioClient));

    public IUserCheckInRepository UserCheckInRepository => _userCheckInRepository.Value;

    private readonly Lazy<IFillInBlankRepository> _fillInBlankRepository =
        new(() => new FillInBlankRepository(seiunDbContext, minioClient));

    public IFillInBlankRepository FillInBlankRepository => _fillInBlankRepository.Value;

    private readonly Lazy<IFillInBlankAnswerRepository> _fillInBlankAnswerRepository =
        new(() => new FillInBlankAnswerRepository(seiunDbContext, minioClient));

    public IFillInBlankAnswerRepository FillInBlankAnswerRepository => _fillInBlankAnswerRepository.Value;

    private readonly Lazy<IFillInBlankWordRepository> _fillInBlankWordRepository =
        new(() => new FillInBlankWordRepository(seiunDbContext, minioClient));

    public IFillInBlankWordRepository FillInBlankWordRepository => _fillInBlankWordRepository.Value;

    private readonly Lazy<IWordDistractorRepository> _wordDistractorRepository =
        new(() => new WordDistractorRepository(seiunDbContext, minioClient));

    public IWordDistractorRepository WordDistractorRepository => _wordDistractorRepository.Value;

    private readonly Lazy<IUserQuestionRepository> _userQuestionRepository =
        new(() => new UserQuestionRepository(seiunDbContext, minioClient));

    public IUserQuestionRepository UserQuestionRepository => _userQuestionRepository.Value;

    private readonly Lazy<IClozeTestRepository> _clozeTestRepository =
        new(() => new ClozeTestRepository(seiunDbContext, minioClient));

    public IClozeTestRepository ClozeTestRepository => _clozeTestRepository.Value;

    private readonly Lazy<IWordBookRepository> _wordBookRepository =
        new(() => new WordBookRepository(seiunDbContext, minioClient));

    public IWordBookRepository WordBookRepository => _wordBookRepository.Value;

    private readonly Lazy<IWordWordBookRepository> _wordWordBookRepository =
        new(() => new WordWordBookRepository(seiunDbContext, minioClient));

    public IWordWordBookRepository WordWordBookRepository => _wordWordBookRepository.Value;
}