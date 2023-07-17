using System.ComponentModel.DataAnnotations;

namespace e_b.Domain.Models.Api.UpdateSettings;

public class PostBody
{
	[MaxLength(32), MinLength(3), RegularExpression(@"^[a-zA-Z0-9\-_]+$", ErrorMessage = "Username can only contain A-Z, a-z, 0-9, - and _")]
	public string? Username { get; set; }

	[MaxLength(512)]
	public string? Description { get; set; }

	// t t t todo
	// [MaxLength(128), RegularExpression(@"^[a-zA-Z0-9\-_\.]{1,128}$", ErrorMessage = "Profile picture must be a valid filename of one of your own content")]
	public string? ProfilePicture { get; set; }
}