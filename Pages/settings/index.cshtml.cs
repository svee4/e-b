using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ViewModels = e_b.Domain.Models.View.settings.index;

namespace e_b.Pages.settings;

public class indexModel : PageModel
{

	public ViewModels.User? User { get; set; }

	private readonly Domain.Models.Database.DatabaseContext _databaseContext;

	public indexModel(Domain.Models.Database.DatabaseContext databaseContext)
	{
		_databaseContext = databaseContext;
	}

	public void OnGet()
	{
		var session = Domain.Models.Session.TryGet(HttpContext);
		if (session is null)
			return;

		var user = _databaseContext.Users.Find(session.UserId);
		if (user is null)
			return;

		User = ViewModels.User.FromDatabase(user);
	}


}

