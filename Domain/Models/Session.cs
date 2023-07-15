using e_b.Domain.Extensions;

namespace e_b.Domain.Models;

public record Session(int UserId, string Username)
{

	/// <summary>
	/// Try to get the active session. If user isnt authenticated, returns null. Does not check for expiration
	/// </summary>
	public static Session? TryGet(HttpContext context)
	{
		ISession session = context.Session;
		string? username = session.GetString("Username");
		int? userId = session.GetInt32("UserId");

		if (userId is null || username.IsNullOrEmpty())
			return null;

		return new Session(userId.Value, username!);
	}

	public static void Set(HttpContext context, int userId, string username)
	{
		ISession session = context.Session;
		session.SetString("Username", username);
		session.SetInt32("UserId", userId);
	}


	public static bool IsAuthenticated(HttpContext context)
	{
		ISession session = context.Session;
		string? username = session.GetString("Username");
		int? userId = session.GetInt32("UserId");

		return userId is not null && !username.IsNullOrEmpty();
	}

}