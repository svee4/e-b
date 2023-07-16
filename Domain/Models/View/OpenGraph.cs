namespace e_b.Domain.Models.View;

public class OpenGraph
{
	public string? Title { get; set; }
	public string? Description { get; set; }
	public string? Image { get; set; }
	public string? Url { get; set; }
	public string? Type { get; set; }

	public int? ImageWidth { get; set; }
	public int? ImageHeight { get; set; }
	public string? ImageAlt { get; set; }

	public Dictionary<string, string> Extra { get; set; } = new Dictionary<string, string>();

	public OpenGraph() { }

	public OpenGraph(string? title, string? description, string? image, string? url, string? type, int? imageWidth, int? imageHeight, string? imageAlt)
	{
		Title = title;
		Description = description;
		Image = image;
		Url = url;
		Type = type;
		ImageWidth = imageWidth;
		ImageHeight = imageHeight;
		ImageAlt = imageAlt;
	}

}