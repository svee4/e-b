namespace e_b.Domain.Models.View.settings.index;

public class User
{
	public string Username { get; set; } = string.Empty;
	public bool CanChangeUsername { get; set; } = false;
	public string Description { get; set; } = string.Empty;
	public string ProfilePicture { get; set; } = string.Empty;

	public static User FromDatabase(Domain.Models.Database.User user) => new User
	{
		Username = user.Username,
		CanChangeUsername = !user.UsernameChanged,
		Description = user.Description ?? string.Empty,
		ProfilePicture = user.ProfilePicture,
	};
}