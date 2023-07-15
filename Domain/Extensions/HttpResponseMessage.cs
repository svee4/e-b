
namespace e_b.Domain.Extensions;

public static class HttpResponseMessageExtensions
{

	public static HttpResponseMessage SetJson(this HttpResponseMessage message, params (string, string)[] content)
	{
		return message.SetJson(content.ToDictionary(x => x.Item1, x => x.Item2));
	}

	public static HttpResponseMessage SetJson(this HttpResponseMessage message, object content)
	{
		message.Content = JsonContent.Create(content);
		message.Content.Headers.ContentType = new("application/json");
		return message;
	}


}