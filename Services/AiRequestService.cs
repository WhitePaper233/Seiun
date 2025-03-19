using OpenAI;
using System.ClientModel;
using OpenAI.Chat;
using RestSharp;
using System.Text.Json;
using Seiun.Entities;
using Seiun.Models.Responses;

namespace Seiun.Services;

public class AIRequestService : IAIRequestService
{
	private readonly IConfigurationRoot _config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("secret.json", optional: false, reloadOnChange: true)
        .Build();

	public async Task<string?> GetAIArticleAsync(List<string> words)
	{
		var apiKey = _config["DeepSeek:ApiKey"];
    	var endpoint = _config["DeepSeek:Endpoint"];

		var prompt = string.Join(",", words);
		var clientOptions = new OpenAIClientOptions
		{
			Endpoint = new Uri($"{endpoint}")
		};
		var clientCredentials = new ApiKeyCredential($"{apiKey}");
		var client = new OpenAIClient(clientCredentials, clientOptions).GetChatClient("deepseek-reasoner");
		var systemPrompt = """
							请根据以下单词，使用逗号分隔，不区分大小写，生成一篇英文文章，帮助学习这些单词。
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
		return completion?.Content[0].Text;
	}

	public async Task<string?> GetAICoverAsync(string aiArticle)
	{
		var apiKey = _config["CA:ApiKey"];
    	var endpoint = _config["CA:Endpoint"];

		aiArticle = aiArticle.Length>900?aiArticle[..900] : aiArticle;

		var client = new RestClient($"{endpoint}");
        var request = new RestRequest
        {
            Method = Method.Post
        };
        request.AddHeader("Authorization", $"Bearer {apiKey}"); 
		request.AddHeader("Content-Type", "application/json");

		var body = new
		{
			prompt = $"根据以下英文文章生成图片，要求阳光，二次元风格。文章：{aiArticle}",
			n = 1,
			model = "dall-e-2",
			size = "512x512"
		};

		request.AddJsonBody(body);

		var response = await client.ExecuteAsync(request); 
		if(response.Content == null)
		{
			return null;
		}
		using var doc = JsonDocument.Parse(response.Content);
		var root = doc.RootElement;
		var dataArray  = root.GetProperty("data");
		if (dataArray .GetArrayLength() > 0)
		{
			var firstElement = dataArray [0];
			var aiCoverUrl = firstElement.GetProperty("url").GetString() ?? string.Empty;
			return aiCoverUrl;
		}
		return null;
	}

	public async Task<FillInBlankInfo?> GetAiFillInBlankAsync(List<string> words)
	{
		var apiKey = _config["DeepSeek:ApiKey"];
		var endpoint = _config["DeepSeek:Endpoint"];
		
		var prompt = string.Join(",", words);
		var clientOptions = new OpenAIClientOptions
		{
			Endpoint = new Uri($"{endpoint}")
		};
		var clientCredentials = new ApiKeyCredential($"{apiKey}");
		var client = new OpenAIClient(clientCredentials, clientOptions).GetChatClient("deepseek-chat");
		var systemPrompt = """
		                   请根据以下单词，使用逗号分隔，不区分大小写，生成一篇英文文章，用于选词填空题目，每个词只填一次，帮我考察巩固这些单词。
		                   题目难度为六级考试选词填空题目难度。
		                   先给出可选单词，在给出英文文章，然后是中文翻译，最后再给出每个空的答案和选择出对应答案的解析。
		                   注意答案的解析一定要合理，可以从词性，词义，句式，翻译，特殊句式等等角度回答，但一定要明确选择某个单词原因。
		                   必须使用 JSON 格式回复

		                   EXAMPLE INPUT:
		                   you,morning

		                   EXAMPLE JSON OUTPUT
		                   {
		                       "type": "选词填空",
		                       "words":[
		                           {
		                               "word": "you"
		                           },
		                           {
		                               "word": "morning"
		                           }
		                       ]
		                       "content": "Good {$1}, what are {$2} doing?",
		                       "transition": "早上好,你正在做什么?"
		                       "answers": [
		                           {
		                               "key": "1",
		                               "answer": "morning",
		                               "analysis": "......"
		                           },
		                           {
		                               "key": "2",
		                               "answer": "you",
		                               "analysis": "......"
		                           }
		                       ]					
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
		if (completion == null)
		{
			return null;
		}

		try
		{
			using var doc = JsonDocument.Parse(completion.Content[0].Text);
			var root = doc.RootElement;
			var wordTexts = root.GetProperty("words").EnumerateArray().Select(w => 
				w.GetProperty("word").GetString() ?? string.Empty).ToList();
			var contentText = root.GetProperty("content").GetString() ?? string.Empty;
			var transitionText = root.GetProperty("transition").GetString() ?? string.Empty;
			var answerTexts = root.GetProperty("answers").EnumerateArray().Select(a =>
				new FillInBlankAnswerInfo
				{
					Key = a.GetProperty("key").GetInt32(),
					Answer = a.GetProperty("answer").GetString() ?? string.Empty,
					Analysis = a.GetProperty("analysis").GetString() ?? string.Empty
				}).ToList();

			return new FillInBlankInfo
			{
				Words = wordTexts,
				Content = contentText,
				Transition = transitionText,
				Answers = answerTexts
			};
		}
		catch
		{
			return null;
		}
	}
}