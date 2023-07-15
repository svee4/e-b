using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace e_b.Pages.authenticate
{
	public class logoutModel : PageModel
	{
		public void OnGet()
		{
			HttpContext.Session.Clear();
			Response.Redirect("/");
		}
	}
}
