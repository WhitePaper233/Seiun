using Microsoft.EntityFrameworkCore;

namespace Seiun.Entities;

public class SeiunDbContext(DbContextOptions<SeiunDbContext> options) : DbContext(options)
{
    public required DbSet<UserEntity> Users { get; init; }
    public required DbSet<ArticleEntity> Articles { get; init; }
    public required DbSet<ArticleLikeEntity> ArticleLikes { get; init; }
    public required DbSet<PublicAnnouncementEntity> PublicAnnouncements {get; init;}
    public required DbSet<CommentEntity> Comments { get; set; }
    public required DbSet<CommentLikeEntity> CommentLike { get; set; }
    public required DbSet<ReplyEntity> Replies { get; set; }
    public required DbSet<WordSessionEntity> Sessions { get; set; }
    public required DbSet<UserTagEntity> UserTag { get; set; }
    public required DbSet<WordEntity> Words { get; set; }
    public required DbSet<ErrorWordRecordEntity> ErrorWords { get; set; }
    public required DbSet<FinishedWordRecordEntity> FinishedWords { get; set; }
    public required DbSet<AiArticleEntity> AiArticles { get; set; }
    public required DbSet<UserCheckInEntity> UserCheckIns { get; set; }
    public required DbSet<FillInBlankEntity> FillInBlanks { get; set; }
    public required DbSet<FillInBlankAnswerEntity> FillInBlankAnswers { get; set; }
    public required DbSet<FillInBlankWordEntity> FillInBlankWords { get; set; }
    public required DbSet<WordDistractorEntity> WordDistractors { get; set; }
    public required DbSet<UserQuestionEntity> UserQuestions { get; set; }
    public required DbSet<ClozeTestEntity> ClozeTests { get; set; }
    public required DbSet<ClozeTestSelectionEntity> ClozeTestSelections { get; set; }
    public required DbSet<ClozeTestAnswerEntity> ClozeTestAnswers { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        # region 添加配置
        
        ConfigureUserEntity(modelBuilder);
        ConfigureArticleEntity(modelBuilder);
        ConfigureArticleLikeEntity(modelBuilder);
        ConfigureAiArticleEntity(modelBuilder);
        ConfigureErrorWordRecordEntity(modelBuilder);
        ConfigureWordSessionEntity(modelBuilder);
        ConfigureUserTagEntity(modelBuilder);
        ConfigureUserCheckInEntity(modelBuilder);
        ConfigureFinishedWordRecordEntity(modelBuilder);
        ConfigureAiArticleEntity(modelBuilder);
        ConfigureUserCheckInEntity(modelBuilder);
        ConfigurePublicAnnouncementEntity(modelBuilder);
        ConfigureReplyEntity(modelBuilder);
        ConfigureUserQuestionEntity(modelBuilder);
        ConfigureWordEntity(modelBuilder);
        ConfigureWordDistractorEntity(modelBuilder);
        
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
        
        // user - UserQuestions
        modelBuilder.Entity<UserEntity>()
            .HasMany(u => u.UserQuestions)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // user - UserTag
        modelBuilder.Entity<UserEntity>()
            .HasMany(u => u.UserTags)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // user - CheckIns
        modelBuilder.Entity<UserEntity>()
            .HasMany(u => u.CheckIns)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
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
        modelBuilder.Entity<ErrorWordRecordEntity>()
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

    private static void ConfigureUserQuestionEntity(ModelBuilder modelBuilder)
    {
        // UserQuestion - user
        modelBuilder.Entity<UserQuestionEntity>()
            .HasOne(q => q.User)
            .WithMany(u => u.UserQuestions)
            .HasForeignKey(q => q.UserId);
    }

    private static void ConfigureUserTagEntity(ModelBuilder modelBuilder)
    {
        // UserTagEntity - User
        modelBuilder.Entity<UserTagEntity>()
            .HasOne(t => t.User)
            .WithMany(u => u.UserTags)
            .HasForeignKey(t => t.UserId);
    }

    private static void ConfigureWordSessionEntity(ModelBuilder modelBuilder)
    {
        // WordSessionEntity - User
        modelBuilder.Entity<WordSessionEntity>()
            .HasOne(s => s.User)
            .WithOne(u => u.WordSession)
            .HasForeignKey<WordSessionEntity>(w => w.UserId);
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
    
    #endregion
}