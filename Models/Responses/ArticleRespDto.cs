using Seiun.Entities;
using Seiun.Resources;
using System.Text.Json.Serialization;

namespace Seiun.Models.Responses;

# region ArticleListResponse

public class ArticleImgDetail
{
    public required string ArticleImgName { get; set; }
}

public sealed class ArticleImgNameResp(int code, string message, ArticleImgDetail? articleImgNameList)
    : BaseRespWithData<ArticleImgDetail>(code, message, articleImgNameList)
{
    public static ArticleImgNameResp Success(string articleImgName)
    {
        return new ArticleImgNameResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.Article.GetArticleImgNameSuccess,
            new ArticleImgDetail
            {
                ArticleImgName = articleImgName
            });
    }

    public static ArticleImgNameResp Fail(int code, string message)
    {
        return new ArticleImgNameResp(code, message, null);
    }
}

/// <summary>
/// 文章列表
/// </summary>
public class ArticleList
{
    public required List<Guid> ArticleIds { get; set; }
}

/// <summary>
/// 文章列表响应
/// </summary>
public sealed class ArticleListResp(int code, string message, ArticleList? articleList)
    : BaseRespWithData<ArticleList>(code, message, articleList)
{
    public static ArticleListResp Success(List<Guid> articleIds)
    {
        return new ArticleListResp(StatusCodes.Status200OK, SuccessMessages.Controller.Article.GetArticleListSuccess,
            new ArticleList
            {
                ArticleIds = articleIds
            });
    }

    public static ArticleListResp Fail(int code, string message)
    {
        return new ArticleListResp(code, message, null);
    }
}

# endregion

# region ArticleDetailResponse

/// <summary>
/// 文章详情
/// </summary>
public class ArticleDetail
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid CreatorId { get; set; }
    public required string Content { get; set; }
    public required string Vocabulary { get; set; }
    public List<string>? ArticleImgUrls { get; set; }
    public string? CoverFileName { get; set; }
    public required long CreateAt { get; set; }
    public required int Like { get; set; }
    public required bool IsPinned { get; set; }
}

public sealed class ArticleDetailResp(int code, string message, ArticleDetail? articleDetail)
    : BaseRespWithData<ArticleDetail>(code, message, articleDetail)
{
    public static ArticleDetailResp Success(ArticleEntity articleEntity, int articleLikedCount)
    {
        var articleImgUrls = articleEntity.ImageFileNames?.Select(imgName => $"/resources/article-image/{imgName}")
            .ToList();
        return new ArticleDetailResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.Article.GetArticleDetailSuccess,
            new ArticleDetail
            {
                Id = articleEntity.Id,
                Title = articleEntity.Title,
                Description = articleEntity.Description ?? "",
                CreatorId = articleEntity.CreatorId,
                Content = articleEntity.Content,
                Vocabulary = articleEntity.Vocabulary,
                ArticleImgUrls = articleImgUrls,
                CoverFileName = articleEntity.CoverFileName,
                CreateAt = articleEntity.CreatedAt.ToUnixTimeSeconds(),
                Like = articleLikedCount,
                IsPinned = articleEntity.IsPinned
            }
        );
    }

    public static ArticleDetailResp Fail(int code, string message)
    {
        return new ArticleDetailResp(code, message, null);
    }
}

# endregion

# region ArticleListResponse

public class ArticleListDto
{
    public required List<ArticleDetail> Articles { get; set; }
    public required int TotalArticle { get; set; }
}

/// <summary>
/// 管理员文章列表响应 
/// </summary>
public sealed class ArticleListResponse(int code, string message, ArticleListDto? articleListDto)
    : BaseRespWithData<ArticleListDto>(code, message, articleListDto)
{
    public static ArticleListResponse Success(ArticleListDto articleListDto)
    {
        return new ArticleListResponse(200, SuccessMessages.Controller.Article.GetArticleListSuccess, articleListDto);
    }

    public static ArticleListResponse Fail(int code, string message)
    {
        return new ArticleListResponse(code, message, null);
    }
}

# endregion

# region AiArticleListResponse

public class AiArticleList
{
    public required List<Guid> AiArticleIds { get; set; }
}

public sealed class AiArticleListResp(int code, string message, AiArticleList? articleList)
    : BaseRespWithData<AiArticleList>(code, message, articleList)
{
    public static AiArticleListResp Success(List<Guid> articleIds)
    {
        return new AiArticleListResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.Article.GetAiArticleListSuccess,
            new AiArticleList
            {
                AiArticleIds = articleIds
            });
    }

    public static AiArticleListResp Fail(int code, string message)
    {
        return new AiArticleListResp(code, message, null);
    }
}

# endregion

# region GetAIArticle

public class AiArticleDetail
{
    public required Guid AiArticleId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Content { get; set; }
    public required string Vocabulary { get; set; }
    public required string CoverFileName { get; set; }
    public required string Tag { get; set; }
}

public sealed class AiArticleDetailResp(int code, string message, AiArticleDetail? aiArticleDetails)
    : BaseRespWithData<AiArticleDetail>(code, message, aiArticleDetails)
{
    public static AiArticleDetailResp Success(AiArticleEntity aiArticleEntity)
    {
        return new AiArticleDetailResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.Article.GetArticleDetailSuccess,
            new AiArticleDetail
            {
                AiArticleId = aiArticleEntity.Id,
                Title = aiArticleEntity.Title,
                Description = aiArticleEntity.Description ?? "",
                Content = aiArticleEntity.Content,
                CoverFileName = aiArticleEntity.CoverFileName,
                Vocabulary = aiArticleEntity.Vocabulary,
                Tag = aiArticleEntity.Tag
            }
        );
    }

    public static AiArticleDetailResp Fail(int code, string message)
    {
        return new AiArticleDetailResp(code, message, null);
    }
}

# endregion

# region Match Ai Article

public class MatchAiArticle
{
    [JsonPropertyName("title")] public required string Title { get; set; }
    [JsonPropertyName("description")] public required string Description { get; set; }
    [JsonPropertyName("content")] public required string Content { get; set; }
    [JsonPropertyName("tag")] public required string Tag { get; set; }
    [JsonPropertyName("vocabulary")] public required string Vocabulary { get; set; }
}

# endregion

# region Match Ai Cover

public class CoverUrl
{
    [JsonPropertyName("url")] public required string Url { get; set; }
}

public class MatchAiArticleCover
{
    [JsonPropertyName("created")] public required int Created { get; set; }
    [JsonPropertyName("data")] public required List<CoverUrl> Data { get; set; }
}

# endregion