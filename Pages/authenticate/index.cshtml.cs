using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using e_b.Domain.Extensions;

namespace e_b.Pages.authenticate
{
	public class indexModel : PageModel
	{
		public string? AuthenticateURL { get; private set; }

		private readonly IConfiguration _config;

		public indexModel(IConfiguration config)
		{
			_config = config;
		}

		public void OnGet()
		{
			string? clientId = _config["ClientId"];
			if (clientId.IsNullOrEmpty())
				throw new Exception("ClientId not configured");

			string? redirectUri = _config["RedirectUri"];
			if (redirectUri.IsNullOrEmpty())
				throw new Exception("redirectUri not configured");

			string scope = "identify";
			string state = Guid.NewGuid().ToString();

			AuthenticateURL = $"https://discord.com/api/oauth2/authorize?response_type=code&prompt=none&client_id={clientId}&redirect_uri={redirectUri}&scope={scope}&state={state}";

			if (Request.Query["fast"] == "1")
				Response.Redirect(AuthenticateURL);
		}
	}
}
