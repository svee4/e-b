namespace e_b.Domain.Extensions;

public static class IEnumerableExtensions
{
	public static string Join<T>(this IEnumerable<T> me, string? separator = null) =>
		string.Join(separator ?? string.Empty, me);
}