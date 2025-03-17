using Minio;
using Seiun.Entities;
using Seiun.Repositories;

namespace Seiun.Services;

public class RepositoryService(SeiunDbContext seiunDbContext, IMinioClient minioClient) : IRepositoryService
{
    private readonly Lazy<IUserRepository> _userRepository = new(() => new UserRepository(seiunDbContext, minioClient));
    public IUserRepository UserRepository => _userRepository.Value;
    private readonly Lazy<IArticleRepository> _articleRepository = new(() => new ArticleRepository(seiunDbContext, minioClient));
    public IArticleRepository ArticleRepository => _articleRepository.Value;

    private readonly Lazy<IArticleLikeRepository> _ArticleLikeRepository = new(() => new ArticleLikeRepository(seiunDbContext, minioClient));
    public IArticleLikeRepository ArticleLikeRepository => _ArticleLikeRepository.Value;

    private readonly Lazy<IPublicAnnouncementRepository> _publicAnnouncementRepository = new(() => new PublicAnnouncementRepository(seiunDbContext, minioClient));
    public IPublicAnnouncementRepository PublicAnnouncementRepository => _publicAnnouncementRepository.Value;
    private readonly Lazy<ICommentRepository> _commentRepository = new(() => new CommentRepository(seiunDbContext, minioClient));
    public ICommentRepository CommentRepository => _commentRepository.Value;

    private readonly Lazy<ICommentLikeRepository> _commentLikeRepository = new(() => new CommentLikeRepository(seiunDbContext, minioClient));
    public ICommentLikeRepository CommentLikeRepository => _commentLikeRepository.Value;

    private readonly Lazy<IReplyRepository> _replyRepository = new(() => new ReplyRepository(seiunDbContext, minioClient));
    public IReplyRepository ReplyRepository => _replyRepository.Value;

    private readonly Lazy<IWordRepository> _wordRepository = new(() => new WordRepository(seiunDbContext, minioClient));
    public IWordRepository WordRepository => _wordRepository.Value;

    private readonly Lazy<ITagRepository> _tagRepository = new(() => new TagRepository(seiunDbContext, minioClient));
    public ITagRepository TagRepository => _tagRepository.Value;

    private readonly Lazy<IUserTagRepository> _userTagRepository = new(() => new UserTagRepository(seiunDbContext, minioClient));
    public IUserTagRepository UserTagRepository => _userTagRepository.Value;

    private readonly Lazy<IWordSessionRepository> _sessionRepository = new(() => new WordSessionRepository(seiunDbContext, minioClient));
    public IWordSessionRepository SessionRepository => _sessionRepository.Value;

    private readonly Lazy<IErrorWordRepository> _errorWordRepository = new(() => new ErrorWordRepository(seiunDbContext, minioClient));
    public IErrorWordRepository ErrorWordRepository => _errorWordRepository.Value;

    private readonly Lazy<IFinishedWordRepository> _finishedWordRepository = new(() => new FinishedWordRepository(seiunDbContext, minioClient));
    public IFinishedWordRepository FinishedWordRepository => _finishedWordRepository.Value;
    
    private readonly Lazy<IAIArticleRepository> _aiArticleRepository = new(() => new AIArticleRepository(seiunDbContext, minioClient));
    public IAIArticleRepository AIArticleRepository => _aiArticleRepository.Value;

    private readonly Lazy<IUserCheckInRepository> _userCheckInRepository = new(() => new UserCheckInRepository(seiunDbContext, minioClient));
    public IUserCheckInRepository UserCheckInRepository => _userCheckInRepository.Value;
    
    private readonly Lazy<IFillInBlankRepository> _fillInBlankRepository = new(() => new FillInBlankRepository(seiunDbContext, minioClient));
    public IFillInBlankRepository FillInBlankRepository => _fillInBlankRepository.Value;
    
    private readonly Lazy<IFillInBlankAnswerRepository> _fillInBlankAnswerRepository = new(() => new FillInBlankAnswerRepository(seiunDbContext, minioClient));
    public IFillInBlankAnswerRepository FillInBlankAnswerRepository => _fillInBlankAnswerRepository.Value;
    
    private readonly Lazy<IFillInBlankWordRepository> _fillInBlankWordRepository = new(() => new FillInBlankWordRepository(seiunDbContext, minioClient));
    public IFillInBlankWordRepository FillInBlankWordRepository => _fillInBlankWordRepository.Value;
}   
