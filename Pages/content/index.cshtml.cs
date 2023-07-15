using e_b.Domain.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ViewModels = e_b.Domain.Models.View.content.index;

namespace e_b.Pages.content;

public class indexModel : PageModel
{
	public bool mNotFound = false;
	public string Username = "";

	public List<ViewModels.Content> mContent = new List<ViewModels.Content>();
	// List<ViewModels.Folder> Folders = new List<ViewModels.Folder>();

	private readonly IConfiguration _config;
	private readonly DatabaseContext _databaseContext;

	public indexModel(IConfiguration config, DatabaseContext databaseContext)
	{
		_config = config;
		_databaseContext = databaseContext;
	}

	public async Task OnGet(string username)
	{
		// TODO: implement folders lol

		var session = Domain.Models.Session.TryGet(HttpContext);
		bool isCurrentUser = session is null ? false : session.Username == username;
		Username = username;

		int userId = await _databaseContext.Users
			.Where(u => u.Username == username)
			.Select(u => u.Id)
			.FirstOrDefaultAsync();

		if (userId == 0)
		{
			Username = "";
			mNotFound = true;
			return;
		}

		var build = _databaseContext.Contents.Where(c => c.OwnerId == userId);
		if (!isCurrentUser)
			build = build.Where(c => !c.Unlisted);

		string? baseDirectory = _config["VirtualContentDirectory"];
		if (baseDirectory is null)
			throw new Exception("VirtualContentDirectory not configured");

		mContent = build
			.Include(c => c.Owner)
			.OrderByDescending(c => c.CreatedAt)
			.Select(c => ViewModels.Content.FromDatabase(c, baseDirectory))
			.ToList();

	}

}
