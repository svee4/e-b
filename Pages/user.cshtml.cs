using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace e_b.Pages
{
	public class userModel : PageModel
	{

		public bool mNotFound = false;
		public string Username = "";
		public string Description = "";
		public string ProfilePicture = "";
		public int ContentCount = 0;

		// inject database
		private readonly Domain.Models.Database.DatabaseContext _databaseContext;

		public userModel(Domain.Models.Database.DatabaseContext databaseContext)
		{
			_databaseContext = databaseContext;
		}

		public async Task OnGet(string username)
		{

			var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Username == username);
			if (user is null)
			{
				mNotFound = true;
				return;
			}

			Username = user.Username;
			Description = user.Description ?? string.Empty;
			ProfilePicture = user.ProfilePicture ?? "/heart.png";
			ContentCount = await _databaseContext.Contents.CountAsync(c => c.OwnerId == user.Id);
		}
	}
}
