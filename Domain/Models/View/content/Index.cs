
namespace e_b.Domain.Models.View.content.index;


public class Folder
{
	public int Id { get; set; } = 0;
	public string Name { get; set; } = "";
	public bool Unlisted { get; set; } = false;
	public DateTime CreatedAt { get; set; } = DateTime.MinValue;

	public List<Content> Content { get; set; } = new List<Content>();

	private Folder() { }

	public static Folder FromDatabase(Domain.Models.Database.Folder folder)
	{
		return new Folder()
		{
			Id = folder.Id,
			Name = folder.Name,
			Unlisted = folder.Unlisted,
			CreatedAt = folder.CreatedAt
		};
	}

}


public class Content
{
	public string Name { get; set; } = "";
	public string Filename { get; set; } = "";
	public string ContentSource { get; set; } = "";
	public DateTime CreatedAt { get; set; } = DateTime.MinValue;
	public Models.Database.Content.ContentType Type { get; set; } = Models.Database.Content.ContentType.Unknown;

	private Content() { }

	public static Content FromDatabase(Domain.Models.Database.Content content, string virtualContentDirectory)
	{
		return new Content()
		{
			Name = content.Name,
			Filename = content.Filename,
			ContentSource = "/" + Path.Combine(virtualContentDirectory, content.Owner.Username, content.Filename),
			Type = content.Type,
			CreatedAt = content.CreatedAt
		};
	}

}