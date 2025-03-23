using OpenAI;
using System.ClientModel;
using OpenAI.Chat;
using RestSharp;
using System.Text.Json;
using Seiun.Controllers;
using Seiun.Entities;
using Seiun.Utils;
using SixLabors.ImageSharp;
using Seiun.Utils.Enums;

namespace Seiun.Services;

public class AiRequestService(IServiceScopeFactory serviceScopeFactory, ILogger<WordSessionController> logger)
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
        var prompt = string.Join(",", words);
        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri("https://api.deepseek.com")
        };
        var clientCredentials = new ApiKeyCredential($"{dApiKey}");
        var client = new OpenAIClient(clientCredentials, clientOptions).GetChatClient("deepseek-reasoner");
        const string systemPrompt = """
                                    请根据以下英文单词，使用逗号分隔，不区分大小写，生成一篇英文文章，帮助学习这些单词。
                                    文章必须使用 Markdown 语法，以markdown文本返回。
                                    文章中也可以使用一些学习的单词的一些词性变换和语法词组，学习的单词和相关语法,词性变换，词组加粗。

                                    EXAMPLE INPUT:
                                    hello,world

                                    EXAMPLE OUTPUT:
                                    # The Beauty of the **World**\n\nIn this vast **world**, a simple **hello** can create new friendships, \n
                                    brighten someone's day, and bring warmth to a lonely heart. 								
                                    """;
        var userPrompt = $"{prompt}";
        var completionOptions = new ChatCompletionOptions
        {
            Temperature = 1.5f,
            ResponseFormat = ChatResponseFormat.CreateTextFormat()
        };
        var messages = new ChatMessage[]
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };
        ChatCompletion completion = await client.CompleteChatAsync(messages, completionOptions);
        var aiArticle = completion.Content[0].Text;
        if (aiArticle == null)
        {
            logger.LogWarning("User {} failed generate ai article", userId);
            return;
        }

        // 生成封面
        aiArticle = aiArticle.Length > 900 ? aiArticle[..900] : aiArticle;

        var coverClient = new RestClient("https://api.chatanywhere.tech/v1/images/generations");
        var coverRequest = new RestRequest
        {
            Method = Method.Post
        };
        coverRequest.AddHeader("Authorization", $"Bearer {cApiKey}");
        coverRequest.AddHeader("Content-Type", "application/json");

        var coverBody = new
        {
            prompt = $"根据以下英文文章生成图片，要求阳光，二次元风格。文章：{aiArticle}",
            n = 1,
            model = "dall-e-2",
            size = "512x512"
        };

        coverRequest.AddJsonBody(coverBody);

        var coverResponse = await coverClient.ExecuteAsync(coverRequest);
        if (coverResponse.Content == null)
        {
            logger.LogWarning("User {} failed generate ai cover", userId);
            return;
        }

        using var doc = JsonDocument.Parse(coverResponse.Content);
        var root = doc.RootElement;
        var dataArray = root.GetProperty("data");
        var firstElement = dataArray[0];
        var aiCoverUrl = firstElement.GetProperty("url").GetString() ?? string.Empty;
        if (aiCoverUrl != string.Empty)
        {
            logger.LogWarning("User {} failed generate ai cover", userId);
            return;
        }

        // 下载图片
        var imageClient = new RestClient(aiCoverUrl);
        var imageRequest = new RestRequest
        {
            Method = Method.Get
        };
        var imageResponse = await imageClient.ExecuteAsync(imageRequest);
        if (imageResponse.RawBytes == null)
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
                await repository.ArticleRepository.UploadArticleImgAsync(processedImageStream,
                    Constants.BucketNames.ArticleCover);
        }
        catch
        {
            logger.LogWarning("User {} failed upload cover image", userId);
            return;
        }

        // 存储ai文章
        Console.WriteLine($"1111111111111111111111111 {aiArticle}");
        var aIArticleEntity = new AiArticleEntity
        {
            UserId = userId,
            SessionId = latestFinishedWordGroup.Key,
            Article = aiArticle,
            CoverUrl = articleImgName
        };
        repository.AiArticleRepository.Create(aIArticleEntity);
        if (!await repository.AiArticleRepository.SaveAsync())
            logger.LogWarning("User {} failed generate ai article", userId);
    }

    // 生成选词填空
    public async Task GenerateAiFillInBlankAsync(List<string> words, Guid userId)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepositoryService>();

        var apiKey = _config["CA:ApiKey"];

        var client = new RestClient("https://api.chatanywhere.tech/v1/chat/completions");
        var request = new RestRequest
        {
            Method = Method.Post
        };
        request.AddHeader("Authorization", $"Bearer {apiKey}");
        request.AddHeader("Content-Type", "application/json");

        var newWords = words.Take(15).ToList();
        var wordText = string.Join(",", newWords);
        const string prompt = """
                              请根据我给出的单词，使用逗号分隔，不区分大小写，生成一篇选词填空题目，每个词只填一次，帮我考察巩固这些单词。
                              返回内容包含一下字段，并严格按照JSON字符串规则，不可有不合JSON规则字符
                              1、我给出的单词,在 words 字段
                              2、选词填空文章，给每个空按顺序编号,并以下划线代替，在 content 字段
                              3、文章中文翻译,在 transition 字段
                              4、每个空的答案，包含每个空的序号,答案以及解析, 在 answers 字段
                              """;

        # region Body

        var body = new
        {
            model = "o1-2024-12-17",
            temperature = 0.6,
            messages = new[]
            {
                new { role = "system", content = $"{prompt}" },
                new { role = "user", content = $"{wordText}" }
            },
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "research_paper_extraction",
                    schema = new
                    {
                        type = "object",
                        properties = new
                        {
                            words = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "string"
                                }
                            },
                            content = new
                            {
                                type = "string"
                            },
                            transition = new
                            {
                                type = "string"
                            },
                            answers = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        key = new
                                        {
                                            type = "integer"
                                        },
                                        answer = new
                                        {
                                            type = "string"
                                        },
                                        analysis = new
                                        {
                                            type = "string"
                                        }
                                    },
                                    required = new[]
                                    {
                                        "key",
                                        "answer",
                                        "analysis"
                                    }
                                }
                            }
                        },
                        required = new[]
                        {
                            "words",
                            "content",
                            "transition",
                            "answers"
                        },
                        additionalProperties = false
                    },
                    strict = true
                }
            }
        };

        # endregion

        request.AddJsonBody(body);
        var response = await client.ExecuteAsync(request);
        if (response.Content == null)
        {
            logger.LogWarning("User {} failed generate ai filled in word book", userId);
            return;
        }

        using var doc = JsonDocument.Parse(response.Content);
        var root = doc.RootElement;

        var questionEntity = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        if (questionEntity == null)
        {
            logger.LogWarning("User {} failed generate ai filled in word book", userId);
            return;
        }

        using var docAgain = JsonDocument.Parse(questionEntity);
        var rootAgain = docAgain.RootElement;

        var wordsElement = rootAgain.GetProperty("words");
        var content = rootAgain.GetProperty("content").GetString();
        var transition = rootAgain.GetProperty("transition").GetString();
        var answersElement = rootAgain.GetProperty("answers");
        if (wordsElement.GetArrayLength() == 0 || content == null || transition == null ||
            answersElement.GetArrayLength() == 0)
        {
            logger.LogWarning("User {} failed generate ai filled in word book", userId);
            return;
        }

        var fillInBlank = new FillInBlankEntity
        {
            Content = content,
            Transition = transition
        };

        var fillInBlankWord = wordsElement.EnumerateArray().Select(w =>
            new FillInBlankWordEntity
            {
                QuestionId = fillInBlank.Id,
                Word = w.GetString() ?? string.Empty
            }).ToList();

        var fillInBlankAnswer = answersElement.EnumerateArray().Select(w =>
            new FillInBlankAnswerEntity
            {
                QuestionId = fillInBlank.Id,
                Key = w.GetProperty("key").GetInt32(),
                Answer = w.GetProperty("answer").GetString() ?? string.Empty,
                Analysis = w.GetProperty("analysis").GetString() ?? string.Empty
            }).ToList();

        repository.FillInBlankRepository.Create(fillInBlank);
        repository.FillInBlankAnswerRepository.BulkAdd(fillInBlankAnswer);
        repository.FillInBlankWordRepository.BulkAdd(fillInBlankWord);
        if (!await repository.FillInBlankWordRepository.SaveAsync() ||
            !await repository.FillInBlankAnswerRepository.SaveAsync() ||
            !await repository.FillInBlankRepository.SaveAsync())
        {
            logger.LogWarning("User {} failed generate ai filled in word book", userId);
            return;
        }

        var userQuestion = new UserChallengeEntity
        {
            UserId = userId,
            ChallengeId = fillInBlank.Id,
            Type = ChallengeType.FillInBlank
        };
        repository.UserChallengeRepository.Create(userQuestion);
        if (!await repository.UserChallengeRepository.SaveAsync())
            logger.LogWarning("User {} failed generate ai filled in word book", userId);
    }

    // 生成完形填空
    public async Task GenerateAiClozeTest(List<string> words, Guid userId)
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

        var clozeTest = new ClozeTestEntity
        {
            ClozeTestJson = completion.Content[0].Text
        };
        

        repository.ClozeTestRepository.Create(clozeTest);
        if (!await repository.ClozeTestRepository.SaveAsync())
        {
            logger.LogWarning("User {} failed generate ai cloze test", userId);
            return;
        }

        var userChallenge = new UserChallengeEntity
        {
            UserId = userId,
            ChallengeId = clozeTest.Id,
            Type = ChallengeType.Cloze
        };
        repository.UserChallengeRepository.Create(userChallenge);
        if (!await repository.UserChallengeRepository.SaveAsync())
            logger.LogWarning("User {} failed generate ai filled in word book", userId);
    }
}