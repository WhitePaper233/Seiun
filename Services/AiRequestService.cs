using OpenAI;
using System.ClientModel;
using OpenAI.Chat;
using RestSharp;
using System.Text.Json;
using Seiun.Controllers;
using Seiun.Entities;
using Seiun.Models.Responses;
using SixLabors.ImageSharp;
using Seiun.Utils.Enums;

namespace Seiun.Services;

public class AiRequestService(IServiceScopeFactory serviceScopeFactory, ILogger<AiRequestService> logger)
    : IAiRequestService
{
    private readonly IConfigurationRoot _config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("secret.json", false, true)
        .Build();


    // 生成文章和封面
    public async Task GenerateAiArticleAsync(Guid userId)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepositoryService>();

        var dApiKey = _config["DeepSeek:ApiKey"];
        var cApiKey = _config["CA:ApiKey"];

        // 获得最新学习单词
        var latestFinishedWordGroup = await repository.FinishedWordRepository.GetLatestFinishedWordIdAsync(userId);
        if (latestFinishedWordGroup == null)
        {
            logger.LogWarning("User {} failed generate ai article", userId);
            return;
        }

        var latestFinishedWordEntities = latestFinishedWordGroup.ToList();
        var latestFinishedWords =
            (await repository.WordRepository.GetByGuidsAsync([.. latestFinishedWordEntities.Select(x => x.WordId)]))
            .ToList();
        var words = latestFinishedWords.Select(x => x.WordText).ToList();

        // 生成文章
        var prompt = string.Join("|", words);
        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri("https://api.deepseek.com")
        };
        var clientCredentials = new ApiKeyCredential($"{dApiKey}");
        var client = new OpenAIClient(clientCredentials, clientOptions).GetChatClient("deepseek-chat");
        const string systemPrompt = """
                                    请根据我提供的使用 | 分隔的英文单词，不区分大小写，生成一篇英文文章，帮助学习这些单词。
                                    title,description,content 都必须使用 Markdown 语法，以markdown文本返回。
                                    文章中也可以使用一些学习的单词的一些词性变换和语法词组，学习的单词和相关语法,词性变换，词组加粗。

                                    EXAMPLE INPUT:
                                    adventure|challenge|journey|explore|courage

                                    EXAMPLE JSON OUTPUT:
                                    {
                                        "title": "The Thrilling Adventure of a Lifetime", 
                                        "description": "An engaging story about a traveler's adventurous journey, using key vocabulary in a natural context.",
                                        "content": "Once upon a time, a young traveler decided to **explore** the mysterious lands beyond his village. He knew that the **journey** ahead would be full of **challenges**, but his **courage** pushed him forward...\n"
                                    }
                                    """;
        var userPrompt = $"{prompt}";
        var completionOptions = new ChatCompletionOptions
        {
            Temperature = 1.5f,
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };
        var messages = new ChatMessage[]
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };
        ChatCompletion completion = await client.CompleteChatAsync(messages, completionOptions);
        var aiArticleJson = completion.Content[0].Text;
        var aiArticle = JsonSerializer.Deserialize<MatchAiArticle>(aiArticleJson);
        if (aiArticle == null)
        {
            logger.LogWarning("User {} failed generate ai article", userId);
            return;
        }

        // 生成封面
        var aiArticleToRequest = aiArticle.Content.Length > 900 ? aiArticle.Content[..900] : aiArticle.Content;

        var coverClient = new RestClient("https://api.chatanywhere.tech/v1/images/generations");
        var coverRequest = new RestRequest
        {
            Method = Method.Post
        };
        coverRequest.AddHeader("Authorization", $"Bearer {cApiKey}");
        coverRequest.AddHeader("Content-Type", "application/json");

        var coverBody = new
        {
            prompt = $"Generate an image based on the following English article: {aiArticleToRequest}",
            n = 1,
            model = "dall-e-3",
            size = "1024x1024"
        };

        coverRequest.AddJsonBody(coverBody);

        var coverResponse = await coverClient.ExecuteAsync<MatchAiArticleCover>(coverRequest);
        if (!coverResponse.IsSuccessful || coverResponse.Data == null)
        {
            logger.LogWarning("User {} failed generate ai cover", userId);
            return;
        }

        var arCoverUrl = coverResponse.Data.Data[0].Url;

        // 下载图片
        var imageClient = new RestClient(arCoverUrl);
        var imageRequest = new RestRequest
        {
            Method = Method.Get
        };
        var imageResponse = await imageClient.ExecuteAsync(imageRequest);
        if (!imageResponse.IsSuccessful || imageResponse.RawBytes == null)
        {
            logger.LogWarning("User {} failed upload cover image", userId);
            return;
        }

        var imageBytes = imageResponse.RawBytes;

        // 处理图片
        string articleImgName;
        try
        {
            await using var imageStream = new MemoryStream(imageBytes);
            var image = await Image.LoadAsync(imageStream);
            await using var processedImageStream = new MemoryStream();
            await image.SaveAsWebpAsync(processedImageStream);
            processedImageStream.Seek(0, SeekOrigin.Begin);
            articleImgName =
                await repository.ArticleRepository.UploadArticleImgAsync(processedImageStream);
        }
        catch
        {
            logger.LogWarning("User {} failed handle cover image", userId);
            return;
        }

        // 存储ai文章
        var aIArticleEntity = new AiArticleEntity
        {
            UserId = userId,
            Title = aiArticle.Title,
            Description = aiArticle.Description,
            Content = aiArticle.Content,
            SessionId = latestFinishedWordGroup.Key,
            CoverFileName = articleImgName
        };
        repository.AiArticleRepository.Create(aIArticleEntity);
        if (!await repository.AiArticleRepository.SaveAsync())
            logger.LogWarning("User {} failed generate ai article", userId);
    }

    // 生成选词填空
    public async Task GenerateAiFillInBlankAsync(List<string> words, Guid userId, Guid sessionId)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepositoryService>();

        var apiKey = _config["CA:ApiKey"];

        var wordText = string.Join("|", words.Take(15));

        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri("https://api.chatanywhere.tech")
        };
        var clientCredentials = new ApiKeyCredential($"{apiKey}");
        var client = new OpenAIClient(clientCredentials, clientOptions).GetChatClient("o3-mini");
        const string systemPrompt = """
                                    我将提供给你几个英语单词，使用｜分隔，请你将这些单词作为考察内容出一篇选词填空题，帮助用户巩固单词记忆。不要出现连续的填空，使用JSON格式回复。
                                    EXAMPLE INPUT: 
                                    abandon|benevolent|courage|diligent|endeavor
                                    EXAMPLE JSON OUTPUT:
                                    {
                                        "type": 2,
                                        "content": "In the pursuit of our dreams, we often face challenges that test our {$1}. Some may choose to {$2}, overwhelmed by difficulties, while others push forward with determination. A {$3} person is not only hardworking but also persistent, ensuring that every effort counts. Throughout history, great leaders have demonstrated {$4} by standing firm in the face of adversity. Their {$5} actions have inspired many to pursue their goals, knowing that success comes from continuous effort and resilience.",
                                        "selections": ["abandon", "benevolent", "courage", "diligent", "endeavor"],
                                        "answers": {
                                            "1": "courage",
                                            "2": "abandon",
                                            "3": "diligent",
                                            "4": "endeavor",
                                            "5": "benevolent"
                                        }
                                    }
                                    """;
        var userPrompt = $"{wordText}";
        var completionOptions = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };
        var messages = new ChatMessage[]
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };
        ChatCompletion completion = await client.CompleteChatAsync(messages, completionOptions);

        var clozeTest = new ChallengeEntity
        {
            UserId = userId,
            SessionId = sessionId,
            Type = ChallengeType.FillInBlank,
            ChallengeJson = completion.Content[0].Text
        };

        repository.ChallengeRepository.Create(clozeTest);
        if (!await repository.ChallengeRepository.SaveAsync())
            logger.LogWarning("User {} failed generate fill in blank test", userId);
    }

    // 生成完形填空
    public async Task GenerateAiClozeTest(List<string> words, Guid userId, Guid sessionId)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepositoryService>();

        var apiKey = _config["CA:ApiKey"];
        var wordText = string.Join('|', words.Take(15));

        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri("https://api.chatanywhere.tech")
        };
        var clientCredentials = new ApiKeyCredential($"{apiKey}");
        var client = new OpenAIClient(clientCredentials, clientOptions).GetChatClient("o3-mini");
        const string systemPrompt = """
                                    我将提供给你几个英语单词，使用｜分隔，请你将这些单词作为考察内容出一篇完型填空题，帮助用户巩固单词记忆。不要出现连续的填空，使用JSON格式回复。
                                    EXAMPLE INPUT: 
                                    hello|world
                                    EXAMPLE JSON OUTPUT:
                                    {
                                        "type": 1,
                                        "content": "The program outputs say {$1} to the {$2}!",
                                        "selections": {
                                            "1":  ["ground", "player", "do", "hello"],
                                            "2": ["back", "judge", "world", "tick"]
                                        },
                                        "answers": {
                                            "1": "hello",
                                            "2": "world",
                                        }
                                    }
                                    """;
        var userPrompt = $"{wordText}";
        var completionOptions = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };
        var messages = new ChatMessage[]
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };
        ChatCompletion completion = await client.CompleteChatAsync(messages, completionOptions);

        var clozeTest = new ChallengeEntity
        {
            UserId = userId,
            SessionId = sessionId,
            Type = ChallengeType.Cloze,
            ChallengeJson = completion.Content[0].Text
        };

        repository.ChallengeRepository.Create(clozeTest);
        if (!await repository.ChallengeRepository.SaveAsync())
            logger.LogWarning("User {} failed generate ai cloze test", userId);
    }
}