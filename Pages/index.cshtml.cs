using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace e_b.Pages;

public class indexModel : PageModel
{
	private readonly ILogger<indexModel> _logger;
	public Domain.Models.Session? Session { get; set; }

	public indexModel(ILogger<indexModel> logger)
	{
		_logger = logger;
	}

	public void OnGet()
	{
		Session = Domain.Models.Session.TryGet(HttpContext)!;
	}
}
