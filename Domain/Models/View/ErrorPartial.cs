namespace e_b.Domain.Models.View;

public class ErrorPartial
{
	public int StatusCode { get; set; }
	public string Message { get; set; }
	public string StatusDescription { get; set; }

	public ErrorPartial() { }
	public ErrorPartial(int statusCode) =>
		StatusCode = statusCode;

	public ErrorPartial(int statusCode, string message) =>
		(StatusCode, Message) = (statusCode, message);

	public ErrorPartial(int statusCode, string message, string statusDescription) =>
		(StatusCode, Message, StatusDescription) = (statusCode, message, statusDescription);

	public static ErrorPartial FromStatusCode(int statusCode) =>
		new ErrorPartial(statusCode, string.Empty, Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(statusCode));

	public static ErrorPartial FromStatusCode(int statusCode, string message) =>
		new ErrorPartial(statusCode, message, Microsoft.AspNetCore.WebUtilities.ReasonPhrases.GetReasonPhrase(statusCode));


}