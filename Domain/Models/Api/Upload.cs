
using System.ComponentModel.DataAnnotations;

namespace e_b.Domain.Models.Api.Upload;

public class PostBody
{
	[Required, MaxLength(128)]
	[RegularExpression(@"^[a-zA-Z0-9\-_]+$", ErrorMessage = "Name can only contain A-Z, a-z, 0-9, - and _")]
	public string Name { get; set; }

	[Required, MaxLength(128)]
	[RegularExpression(@"^[a-zA-Z0-9\-_\.]+$", ErrorMessage = "Filename can only contain A-Z, a-z, 0-9, -, _ and .")]
	public string Filename { get; set; }

	[MaxLength(512), MinLength(1)]
	public string? Description { get; set; }

	[Required, ContentLength(25 * 1024 * 1024)]
	public IFormFile File { get; set; }

	public bool Unlisted { get; set; } = false;

	[Required]
	public Models.Database.Content.ContentType Type { get; set; }

	public int? Width { get; set; }
	public int? Height { get; set; }
}

