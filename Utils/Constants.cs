using Microsoft.IdentityModel.Tokens;

namespace Seiun.Utils;

public static class Constants
{
    public static class Email
    {
        public const int MaxLength = 256;
    }

    public static class User
    {
        public const int MaxDescriptionLength = 256;
        public const int MaxUserNameLength = 16;
        public const int MaxNickNameLength = 16;
        public const int MaxPhoneNumberLength = 15;
        public const int MaxAvatarSize = 8 * 1024 * 1024; // 8MB
        public const int AvatarMaxWidth = 1024;
        public const int AvatarMaxHeight = 1024;
        public const int AvatarStorageSize = 256;
        public static readonly string[] AllowedAvatarExtensions = [".jpg", ".jpeg", ".png", "webp"];
    }

    public static class Token
    {
        public const int TokenExpirationTime = 7 * 24; // 7 days
        public const string SignAlgorithm = SecurityAlgorithms.HmacSha256;
    }

    public static class BucketNames
    {
        public const string Avatar = "avatars";
        public const string ArticleImages = "article-images";
        public const string ArticleCover = "article-covers";
    }

    public static class Article
    {
        public const int MaxArticleImageSize = 8 * 1024 * 1024; // 8MB
        public const int ArticleImageMaxWidth = 3 * 1024;
        public const int ArticleImageMaxHeight = 3 * 1024;
        public const int ArticleImgStorageWidth = 512;
        public static readonly string[] AllowedArticleImageExtensions = [".jpg", ".jpeg", ".png", "webp"];
        public const int MaxArticleCoverSize = 5 * 1024 * 1024;
        public const int MaxArticleCoverHeight = 1024;
        public const int MaxArticleCoverWidth = 1024;
        public const int ArticleCoverStorageSize = 256;
        public const int MaxArticleLength = 5000;
        public const int MaxCoverUrlLength = 1000;
    }

    public static class PublicAnnotation
    {
        public const int MaxAnnotationLength = 500;
    }

    public static class Word
    {
        public const int MaxTagNameLength = 10;
        public const int MaxWordTextLength = 50;
        public const int MaxWordPronunciationLength = 50;
        public const int MaxWordDefinitionLength = 50;
    }
}