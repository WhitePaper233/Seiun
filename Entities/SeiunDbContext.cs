using Microsoft.EntityFrameworkCore;
using Seiun.Utils.Interceptors;

namespace Seiun.Entities;

public class SeiunDbContext(DbContextOptions<SeiunDbContext> options) : DbContext(options)
{
	public required DbSet<UserEntity> Users { get; set; }
	public required DbSet<ArticleEntity> Articles { get; set; }
	public required DbSet<ArticleLikeEntity> ArticleLikes { get; set; }
	public required DbSet<PublicAnnouncementEntity> PublicAnnouncements { get; set; }
	public required DbSet<CommentEntity> Comments { get; set; }
	public required DbSet<CommentLikeEntity> CommentLike { get; set; }
	public required DbSet<ReplyEntity> Replies { get; set; }
	public required DbSet<WordSessionEntity> Sessions { get; set; }
	public required DbSet<UserPlanEntity> UserPlans { get; set; }
	public required DbSet<WordEntity> Words { get; set; }
	public required DbSet<WrongWordRecordEntity> WrongWords { get; set; }
	public required DbSet<FinishedWordRecordEntity> FinishedWords { get; set; }
	public required DbSet<AiArticleEntity> AiArticles { get; set; }
	public required DbSet<UserCheckInEntity> UserCheckIns { get; set; }

	public required DbSet<WordDistractorEntity> WordDistractors { get; set; }
	public required DbSet<ChallengeEntity> Challenges { get; set; }

	public required DbSet<WordBookEntity> WordBooks { get; set; }
	public required DbSet<WordWordBookEntity> WordWordBooks { get; set; }

	public required DbSet<MistakeBookEntity> MistakeBook { get; set; }

	override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		#region 拦截器

		optionsBuilder.AddInterceptors(new TimeStampInterceptor());
		base.OnConfiguring(optionsBuilder);

		#endregion
	}

	override protected void OnModelCreating(ModelBuilder modelBuilder)
	{
		# region 添加配置

		ConfigureUserEntity(modelBuilder);
		ConfigureArticleEntity(modelBuilder);
		ConfigureArticleLikeEntity(modelBuilder);
		ConfigureAiArticleEntity(modelBuilder);
		ConfigureErrorWordRecordEntity(modelBuilder);
		ConfigureWordSessionEntity(modelBuilder);
		ConfigureUserPlansEntity(modelBuilder);
		ConfigureUserCheckInEntity(modelBuilder);
		ConfigureFinishedWordRecordEntity(modelBuilder);
		ConfigureAiArticleEntity(modelBuilder);
		ConfigureUserCheckInEntity(modelBuilder);
		ConfigurePublicAnnouncementEntity(modelBuilder);
		ConfigureReplyEntity(modelBuilder);
		ConfigureWordEntity(modelBuilder);
		ConfigureWordDistractorEntity(modelBuilder);
		ConfigureWordWordBookEntity(modelBuilder);
		ConfigureChallengeEntity(modelBuilder);
		ConfigureMistakeBookEntity(modelBuilder);

		modelBuilder.Entity<WordWordBookEntity>().HasKey(p => new { p.WordId, p.BookId });

		base.OnModelCreating(modelBuilder);

		# endregion
	}

	# region 配置导航和级联删除

	private static void ConfigureUserEntity(ModelBuilder modelBuilder)
	{
		// User - Article
		modelBuilder.Entity<UserEntity>()
			.HasMany(u => u.Articles)
			.WithOne(a => a.Creator)
			.HasForeignKey(a => a.CreatorId)
			.OnDelete(DeleteBehavior.Cascade);

		// User - AiArticle
		modelBuilder.Entity<UserEntity>()
			.HasMany(u => u.AiArticles)
			.WithOne(a => a.User)
			.HasForeignKey(a => a.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		// user - ErrorWordRecord
		modelBuilder.Entity<UserEntity>()
			.HasMany(u => u.ErrorWordRecords)
			.WithOne(e => e.User)
			.HasForeignKey(e => e.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		// user - FinishedWordRecord
		modelBuilder.Entity<UserEntity>()
			.HasMany(u => u.FinishedWordRecords)
			.WithOne(f => f.User)
			.HasForeignKey(f => f.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		// user - PublicAnnouncements
		modelBuilder.Entity<UserEntity>()
			.HasMany(u => u.PublicAnnouncements)
			.WithOne(p => p.Admin)
			.HasForeignKey(p => p.AdminId)
			.OnDelete(DeleteBehavior.Cascade);

		// user - UserPlans
		modelBuilder.Entity<UserEntity>()
			.HasMany(u => u.UserPlans)
			.WithOne(u => u.User)
			.HasForeignKey(u => u.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		// user - CheckIns
		modelBuilder.Entity<UserEntity>()
			.HasMany(u => u.CheckIns)
			.WithOne(c => c.User)
			.HasForeignKey(c => c.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		// user - session
		modelBuilder.Entity<UserEntity>()
			.HasMany(u => u.WordSession)
			.WithOne(w => w.User)
			.HasForeignKey(w => w.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		// user - MistakeBook
		modelBuilder.Entity<UserEntity>()
			.HasMany(u => u.MistakeBook)
			.WithOne(m => m.User)
			.HasForeignKey(m => m.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}

	private static void ConfigureArticleEntity(ModelBuilder modelBuilder)
	{
		// Article - user
		modelBuilder.Entity<ArticleEntity>()
			.HasOne(a => a.Creator)
			.WithMany(u => u.Articles)
			.HasForeignKey(a => a.CreatorId);

		// Article - Like
		modelBuilder.Entity<ArticleEntity>()
			.HasMany(a => a.Likes)
			.WithOne(l => l.LikedArticle)
			.HasForeignKey(l => l.LikedArticleId)
			.OnDelete(DeleteBehavior.Cascade);

		// Article - Comments
		modelBuilder.Entity<ArticleEntity>()
			.HasMany(a => a.Comments)
			.WithOne(c => c.Article)
			.HasForeignKey(c => c.PostId)
			.OnDelete(DeleteBehavior.Cascade);
	}

	private static void ConfigureArticleLikeEntity(ModelBuilder modelBuilder)
	{
		// ArticleLike - Article
		modelBuilder.Entity<ArticleLikeEntity>()
			.HasOne(a => a.LikedArticle)
			.WithMany(a => a.Likes)
			.HasForeignKey(a => a.LikedArticleId);
	}

	private static void ConfigureAiArticleEntity(ModelBuilder modelBuilder)
	{
		// AiArticle - User
		modelBuilder.Entity<AiArticleEntity>()
			.HasOne(a => a.User)
			.WithMany(u => u.AiArticles)
			.HasForeignKey(a => a.UserId);
	}

	private static void ConfigureErrorWordRecordEntity(ModelBuilder modelBuilder)
	{
		// ErrorWordRecordEntity - user
		modelBuilder.Entity<WrongWordRecordEntity>()
			.HasOne(e => e.User)
			.WithMany(u => u.ErrorWordRecords)
			.HasForeignKey(u => u.UserId);
	}

	private static void ConfigureFinishedWordRecordEntity(ModelBuilder modelBuilder)
	{
		// FinishedWordRecordEntity - user
		modelBuilder.Entity<FinishedWordRecordEntity>()
			.HasOne(e => e.User)
			.WithMany(u => u.FinishedWordRecords)
			.HasForeignKey(u => u.UserId);
	}

	private static void ConfigurePublicAnnouncementEntity(ModelBuilder modelBuilder)
	{
		// PublicAnnouncementEntity - user
		modelBuilder.Entity<PublicAnnouncementEntity>()
			.HasOne(a => a.Admin)
			.WithMany(u => u.PublicAnnouncements)
			.HasForeignKey(a => a.AdminId);
	}

	private static void ConfigureReplyEntity(ModelBuilder modelBuilder)
	{
		// ReplyEntity - comment
		modelBuilder.Entity<ReplyEntity>()
			.HasOne(r => r.Comment)
			.WithMany(c => c.Replies)
			.HasForeignKey(r => r.CommentId);
	}

	private static void ConfigureUserCheckInEntity(ModelBuilder modelBuilder)
	{
		// UserCheckInEntity - User
		modelBuilder.Entity<UserCheckInEntity>()
			.HasOne(c => c.User)
			.WithMany(u => u.CheckIns)
			.HasForeignKey(c => c.UserId);
	}

	private static void ConfigureUserPlansEntity(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<UserPlanEntity>()
			.HasOne(t => t.User)
			.WithMany(u => u.UserPlans)
			.HasForeignKey(t => t.UserId);
	}

	private static void ConfigureWordSessionEntity(ModelBuilder modelBuilder)
	{
		// WordSessionEntity - User
		modelBuilder.Entity<WordSessionEntity>()
			.HasOne(s => s.User)
			.WithMany(u => u.WordSession)
			.HasForeignKey(s => s.UserId);

		// WordSessionEntity - clozeTest
		modelBuilder.Entity<WordSessionEntity>()
			.HasMany(s => s.Challenges)
			.WithOne(c => c.Session)
			.HasForeignKey(c => c.SessionId)
			.OnDelete(DeleteBehavior.Cascade);
	}

	private static void ConfigureWordEntity(ModelBuilder modelBuilder)
	{
		// WordEntity - WordDistractorEntity
		modelBuilder.Entity<WordEntity>()
			.HasMany(w => w.WordDistractors)
			.WithOne(d => d.Word)
			.HasForeignKey(d => d.WordId)
			.OnDelete(DeleteBehavior.Cascade);
	}

	private static void ConfigureWordDistractorEntity(ModelBuilder modelBuilder)
	{
		// WordDistractorEntity - wordEntity
		modelBuilder.Entity<WordDistractorEntity>()
			.HasOne(d => d.Word)
			.WithMany(w => w.WordDistractors)
			.HasForeignKey(d => d.WordId);
	}

	private static void ConfigureWordWordBookEntity(ModelBuilder modelBuilder)
	{
		// WordBookWordBook - word
		modelBuilder.Entity<WordWordBookEntity>()
			.HasOne(b => b.Word)
			.WithMany(b => b.Books)
			.HasForeignKey(b => b.WordId);

		// WordBookWordBook - book
		modelBuilder.Entity<WordWordBookEntity>()
			.HasOne(b => b.Book)
			.WithMany(b => b.Words)
			.HasForeignKey(b => b.BookId);
	}

	private static void ConfigureChallengeEntity(ModelBuilder modelBuilder)
	{
		// ChallengeEntity - session
		modelBuilder.Entity<ChallengeEntity>()
			.HasOne(c => c.Session)
			.WithMany(s => s.Challenges)
			.HasForeignKey(c => c.SessionId);
	}

	private static void ConfigureMistakeBookEntity(ModelBuilder modelBuilder)
	{
		// MistakeBook - user
		modelBuilder.Entity<MistakeBookEntity>()
			.HasOne(m => m.User)
			.WithMany(u => u.MistakeBook)
			.HasForeignKey(m => m.UserId);
	}

	#endregion
}
