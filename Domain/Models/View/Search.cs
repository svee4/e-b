namespace e_b.Domain.Models.View.search;

public class ResultItem
{
	public string Username { get; set; } = "";
	public string ContentName { get; set; } = "";
	public string ContentFilename { get; set; } = "";
	public string ContentSource { get; set; } = "";
	public Domain.Models.Database.Content.ContentType ContentType { get; set; } = Domain.Models.Database.Content.ContentType.Unknown;
	public DateTime CreatedAt { get; set; } = DateTime.MinValue;

	public static ResultItem FromDatabase(Domain.Models.Database.Content content, string virtualContentDirectory)
		=> new()
		{
			Username = content.Owner.Username,
			ContentName = content.Name,
			ContentFilename = content.Filename,
			ContentSource = "/" + Path.Combine(virtualContentDirectory, content.Owner.Username, content.Filename),
			ContentType = content.Type,
			CreatedAt = content.CreatedAt
		};

}


public class SearchData
{
	public string Name { get; set; } = "";
	public string Username { get; set; } = "";
	public string Type { get; set; } = "";
	public string OrderBy { get; set; } = "";
	public string SortBy { get; set; } = "";
}