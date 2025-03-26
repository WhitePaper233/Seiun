using Seiun.Entities;
using Seiun.Resources;

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
    public required Guid CreatorId { get; set; }
    public required string Article { get; set; }
    public List<string>? ArticleImgUrls { get; set; }
    public string? CoverFileName { get; set; }
    public required DateTimeOffset CreateAt { get; set; }
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
                CreatorId = articleEntity.CreatorId,
                Article = articleEntity.Article,
                ArticleImgUrls = articleImgUrls,
                CoverFileName = articleEntity.CoverFileName,
                CreateAt = articleEntity.CreatedAt,
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

/// <summary>
/// 所有文章响应
/// </summary>
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

# region GetAIArticle

public class AiArticleList
{
    public required List<AiArticleDetail> AiArticles { get; set; }
}

public class AiArticleDetail
{
    public required string AiArticle { get; set; }
    public required string AiCoverUrl { get; set; }
}

public sealed class AiArticleDetailResp(int code, string message, AiArticleList? aiArticleDetails)
    : BaseRespWithData<AiArticleList>(code, message, aiArticleDetails)
{
    public static AiArticleDetailResp Success(List<AiArticleEntity> aiArticleEntities)
    {
        return new AiArticleDetailResp(StatusCodes.Status200OK,
            SuccessMessages.Controller.Article.GetArticleDetailSuccess,
            new AiArticleList
            {
                AiArticles = aiArticleEntities.Select(aiArticleEntity =>
                    new AiArticleDetail
                    {
                        AiArticle = aiArticleEntity.Article, AiCoverUrl = aiArticleEntity.CoverUrl
                    }).ToList()
            }
        );
    }

    public static AiArticleDetailResp Fail(int code, string message)
    {
        return new AiArticleDetailResp(code, message, null);
    }
}

# endregion