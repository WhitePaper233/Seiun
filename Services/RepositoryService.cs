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

	private readonly Lazy<IArticleLikeRepository> _articleLikeRepository =
		new(() => new ArticleLikeRepository(seiunDbContext, minioClient));

	public IArticleLikeRepository ArticleLikeRepository => _articleLikeRepository.Value;

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

	private readonly Lazy<IWrongWordRepository> _errorWordRepository =
		new(() => new WrongWordRepository(seiunDbContext, minioClient));

	public IWrongWordRepository WrongWordRepository => _errorWordRepository.Value;

	private readonly Lazy<IFinishedWordRepository> _finishedWordRepository =
		new(() => new FinishedWordRepository(seiunDbContext, minioClient));

	public IFinishedWordRepository FinishedWordRepository => _finishedWordRepository.Value;

	private readonly Lazy<IAiArticleRepository> _aiArticleRepository =
		new(() => new AiArticleRepository(seiunDbContext, minioClient));

	public IAiArticleRepository AiArticleRepository => _aiArticleRepository.Value;

	private readonly Lazy<IUserCheckInRepository> _userCheckInRepository =
		new(() => new UserCheckInRepository(seiunDbContext, minioClient));

	public IUserCheckInRepository UserCheckInRepository => _userCheckInRepository.Value;


	private readonly Lazy<IWordDistractorRepository> _wordDistractorRepository =
		new(() => new WordDistractorRepository(seiunDbContext, minioClient));

	public IWordDistractorRepository WordDistractorRepository => _wordDistractorRepository.Value;


	private readonly Lazy<IChallengeRepository> _clozeTestRepository =
		new(() => new ChallengeRepository(seiunDbContext, minioClient));

	public IChallengeRepository ChallengeRepository => _clozeTestRepository.Value;

	private readonly Lazy<IWordBookRepository> _wordBookRepository =
		new(() => new WordBookRepository(seiunDbContext, minioClient));

	public IWordBookRepository WordBookRepository => _wordBookRepository.Value;

	private readonly Lazy<IWordWordBookRepository> _wordWordBookRepository =
		new(() => new WordWordBookRepository(seiunDbContext, minioClient));

	public IWordWordBookRepository WordWordBookRepository => _wordWordBookRepository.Value;

	private readonly Lazy<IMistakeBookRepository> _mistakeBookRepository =
		new(() => new MistakeBookRepository(seiunDbContext, minioClient));

	public IMistakeBookRepository MistakeBookRepository => _mistakeBookRepository.Value;
}
