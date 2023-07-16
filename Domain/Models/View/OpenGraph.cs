namespace e_b.Domain.Models.View;

public class OpenGraph
{
	public string Title { get; set; }
	public string Description { get; set; }
	public string Image { get; set; }
	public string Url { get; set; }
	public string Type { get; set; }

	public OpenGraph() { }

	public OpenGraph(string title, string description, string image, string url, string type)
	{
		Title = title;
		Description = description;
		Image = image;
		Url = url;
		Type = type;
	}

}