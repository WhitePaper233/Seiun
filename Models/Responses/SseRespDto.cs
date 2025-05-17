using System.Text.Json.Serialization;
using System.Text;
using Seiun.Entities;

namespace Seiun.Models.Responses;

#region Match Extract Words

public class MatchExtractWords
{
	[JsonPropertyName("words")] public required List<string> Words { get; set; }
}

public class ExtractWordDetails
{
	public required List<WordEntity> WordDetails { get; set; }
}

public class SseResponse
{
	public static async Task SseResp(HttpResponse httpResp, string respString, CancellationToken cancellationToken = default)
	{
		// CancellationToken 监听客户端连接，如果连接断开，则CancellationToken.isCancellationRequested为true
		// 取消发送
		var messageData = $"data: {respString}\n\n";
		var messageBytes = Encoding.UTF8.GetBytes(messageData);
		var buffer = new ReadOnlyMemory<byte>(messageBytes, 0, messageBytes.Length);
		await httpResp.Body.WriteAsync(buffer, cancellationToken);
		await httpResp.Body.FlushAsync(cancellationToken);
		await Task.Delay(200, cancellationToken);
	}
}

#endregion
