namespace Seiun.Models.Responses;

public class BaseResp(int code, string message)
{
	public int Code { get; set; } = code;
	public string Message { get; set; } = message;
}

public class BaseRespWithData<T>(int code, string message, T? data) : BaseResp(code, message)
{
	public T? Data { get; set; } = data;
}

public static class ResponseFactory
{
	public static BaseResp NewSuccessBaseResponse(string? message)
	{
		message ??= string.Empty;
		return new BaseResp(200, message);
	}

	public static BaseResp NewFailedBaseResponse(int code, string? message)
	{
		message ??= string.Empty;
		return new BaseResp(code, message);
	}
}
