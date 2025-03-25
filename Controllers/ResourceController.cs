using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Minio.Exceptions;
using Seiun.Services;
using Seiun.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Seiun.Utils.Enums;

namespace Seiun.Controllers;

[ApiController]
[Route("/resources")]
public class ResourceController(ILogger<UserController> logger, IRepositoryService repository)
    : ControllerBase
{
    /// <summary>
    /// 头像文件接口
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="size">尺寸参数</param>
    /// <returns>头像文件</returns>
    [HttpGet("avatar/{fileName}")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvatar(string fileName, [FromQuery] int size = 256)
    {
        if (string.IsNullOrWhiteSpace(fileName) || size <= 0) return BadRequest();

        MemoryStream avatarStream;
        try
        {
            avatarStream = await repository.UserRepository.GetAvatarAsync(fileName);
        }
        catch (MinioException e)
        {
            if (e is ObjectNotFoundException) return NotFound();

            logger.LogError(e, "Failed to get avatar: {}", fileName);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        // 如果不需要调整大小，直接返回原图
        if (size >= Constants.User.AvatarStorageSize) return File(avatarStream, MediaTypeNames.Image.Webp);

        try
        {
            // 调整图像大小
            using var image = await Image.LoadAsync(avatarStream);
            image.Mutate(ipc => ipc.Resize(size, size));

            // 保存为 webp 格式
            var ms = new MemoryStream();
            await image.SaveAsWebpAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            // 返回调整后的图像
            return File(ms, MediaTypeNames.Image.Webp);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to resize avatar: {}", fileName);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// 文章图片接口
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns>文章图片文件</returns>
    [HttpGet("article-image/{fileName}")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetArticleImages(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return BadRequest();

        MemoryStream articleImgStream;
        try
        {
            articleImgStream =
                await repository.ArticleRepository.GetArticleImgAsync(fileName, Constants.BucketNames.ArticleImages);
        }
        catch (MinioException e)
        {
            if (e is ObjectNotFoundException) return NotFound();

            logger.LogError(e, "Failed to get article images: {}", fileName);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return File(articleImgStream, MediaTypeNames.Image.Webp);
    }

    /// <summary>
    /// 文章封面接口
    /// </summary>
    /// <param name="fileName">文件URL</param>
    /// <param name="width">图片宽度</param>>
    /// <returns></returns>
    [HttpGet("article-cover/{fileName}")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetArticleCover(string fileName, [FromQuery] int width = 0)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return BadRequest();

        MemoryStream articleCoverStream;
        try
        {
            articleCoverStream =
                await repository.ArticleRepository.GetArticleImgAsync(fileName, Constants.BucketNames.ArticleCover);
        }
        catch (MinioException e)
        {
            if (e is ObjectNotFoundException) return NotFound();

            logger.LogError(e, "Failed to get article images: {}", fileName);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (width <= 0) return File(articleCoverStream, MediaTypeNames.Image.Webp);

        try
        {
            // 调整图像大小
            using var image = await Image.LoadAsync(articleCoverStream);
            image.Mutate(ipc => ipc.Resize(width, 0));

            // 保存为 webp 格式
            var ms = new MemoryStream();
            await image.SaveAsWebpAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            // 返回调整后的图像
            return File(ms, MediaTypeNames.Image.Webp);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to resize article cover: {}", fileName);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// 单词助记图片接口
    /// </summary>
    /// <param name="wordId">单词Id</param>
    /// <param name="height">设置图片高度</param>
    /// <param name="width">设置图片宽度</param>
    /// <returns>图片文件</returns>
    [HttpGet("word-image/{wordId:guid}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWordMnemonicImage(Guid wordId, [FromQuery] int height = 0,
        [FromQuery] int width = 0)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden);


        var wordText = (await repository.WordRepository.GetByIdAsync(wordId))?.WordText;
        if (wordText == null)
            return NotFound();

        var fileName = $"{wordText}.webp";
        MemoryStream wordImgStream;
        try
        {
            wordImgStream = await repository.WordRepository.GetWordMnemonicImage(fileName);
        }
        catch (Exception e)
        {
            if (e is ObjectNotFoundException) return NotFound();

            logger.LogError(e, "Failed to get word mnemonic image: {}", fileName);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (height <= 0 || width <= 0) return File(wordImgStream, MediaTypeNames.Image.Webp);

        try
        {
            // 调整图像大小
            using var image = await Image.LoadAsync(wordImgStream);
            image.Mutate(ipc => ipc.Resize(width, height));

            // 保存为 webp 格式
            var ms = new MemoryStream();
            await image.SaveAsWebpAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            // 返回调整后的图像
            return File(ms, MediaTypeNames.Image.Webp);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to resize word mnemonic image: {}", fileName);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// 获取单词音频接口
    /// </summary>
    /// <param name="wordId">单词Id</param>
    /// <returns>音频文件</returns>
    [HttpGet("word-audio/{wordId:guid}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles =
        $"{nameof(UserRole.User)},{nameof(UserRole.Creator)},{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWordAudio(Guid wordId)
    {
        var userId = User.GetUserId();
        if (userId == null)
            return StatusCode(StatusCodes.Status403Forbidden);


        var wordText = (await repository.WordRepository.GetByIdAsync(wordId))?.WordText;
        if (wordText == null)
            return NotFound();

        var fileName = $"{wordText}.mp3";
        var wordAudioStream = await repository.WordRepository.GetWordAudio(fileName);

        return File(wordAudioStream, "audio/mpeg");
    }
}