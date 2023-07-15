namespace e_b.Domain.Extensions;

public static class StringExtensions
{
	public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);
	public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);

	public static byte[]? FromBase64(this string str, bool fast = false)
	{
		if (fast) return Convert.FromBase64String(str);

		try
		{
			return Convert.FromBase64String(str);
		}
		catch
		{
			return null;
		}
	}
}

