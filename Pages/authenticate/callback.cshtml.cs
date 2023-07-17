
using Microsoft.AspNetCore.Mvc.RazorPages;
using e_b.Domain.Extensions;
using System.Text.Json;
using System.Net.Http.Headers;
using e_b.Domain.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace e_b.Pages.authenticate
{
	public class callbackModel : PageModel
	{

		public string? ErrorMessage { get; private set; }


		private static HttpClient _httpClient = new HttpClient();

		private readonly IConfiguration _config;
		private readonly DatabaseContext _databaseContext;

		public callbackModel(IConfiguration config, DatabaseContext databaseContext)
		{
			_config = config;
			_databaseContext = databaseContext;
		}

		public async Task OnGet()
		{

			string? code = Request.Query["code"];
			if (code.IsNullOrEmpty())
			{
				ErrorMessage = "Missing required parameter code";
				return;
			}

			string? clientId = _config["ClientId"];
			if (clientId.IsNullOrEmpty())
				throw new Exception("ClientId not configured");

			string? clientSecret = _config["ClientSecret"];
			if (clientSecret.IsNullOrEmpty())
				throw new Exception("ClientSecret not configured");

			string? redirectUri = _config["RedirectUri"];
			if (redirectUri.IsNullOrEmpty())
				throw new Exception("RedirectUri not configured");

			TokenResponse? data;
			// get access token

			{

				using HttpRequestMessage request = new(HttpMethod.Post, "https://discord.com/api/oauth2/token");
				request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
				{
					{ "client_id", clientId! },
					{ "client_secret", clientSecret! },
					{ "grant_type", "authorization_code" },
					{ "code", code! },
					{ "redirect_uri", redirectUri! }
				});

				var result = await _httpClient.SendAsync(request);

				string? json = await result.Content.ReadAsStringAsync();
				if (!result.IsSuccessStatusCode || json.IsNullOrEmpty())
				{
					ErrorMessage = $"Failed to get access token {result.StatusCode}: '{json}'";
					return;
				}


				try
				{
					data = JsonSerializer.Deserialize<TokenResponse>(json);
				}
				catch (Exception ex)
				{
					ErrorMessage = $"Failed to parse access token: {ex.Message}";
					return;
				}

				if (data is null)
				{
					ErrorMessage = "Failed to parse access token (unknown error)";
					return;
				}
			}

			UserResponse? user;
			// get user info
			{
				using HttpRequestMessage request = new(HttpMethod.Get, "https://discord.com/api/v10/users/@me");
				request.Headers.Authorization = new AuthenticationHeaderValue(data.token_type, data.access_token);
				var response = await _httpClient.SendAsync(request);

				string? json = await response.Content.ReadAsStringAsync();
				if (!response.IsSuccessStatusCode || json.IsNullOrEmpty())
				{
					ErrorMessage = $"Failed to get user object {response.StatusCode}: '{json}'";
					return;
				}

				try
				{
					user = JsonSerializer.Deserialize<UserResponse>(json);
				}
				catch (Exception ex)
				{
					ErrorMessage = $"Failed to parse user object: {ex.Message}";
					return;
				}

				if (user is null)
				{
					ErrorMessage = "Failed to parse user object (unknown error)";
					return;
				}
			}

			// DOESNT WORK BTW
			string? allowedIds = _config["AllowedIds"];
			if (!allowedIds.IsNullOrEmpty())
			{
				string[] ids = allowedIds!.Split(',');
				if (!ids.Contains(user.id))
				{
					ErrorMessage = "User id not allowed. This website is currently in closed beta, Please contact admin to get access or wait for public release";
					return;
				}
			}

			Domain.Models.Database.User? dbUser = await _databaseContext.Users.FirstOrDefaultAsync(u => u.DiscordUserId == user.id);
			if (dbUser is null)
			{
				// insert new user info

				string username = user.username;
				if (username.Length < 3) username = username + Random.Shared.Next(10, 99);

				int i = 0;
				while (await _databaseContext.Users.AnyAsync(u => u.Username == username))
				{
					if (i++ > 10) throw new Exception("Is this real life?");

					// this might be top 5 worst things ive ever written 
					if (username.Length >= 32)
					{
						const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-";
						char[] buffer = new char[32];
						for (int j = 0; j < buffer.Length; j++)
							buffer[j] = chars[Random.Shared.Next(chars.Length)];

						username = new string(buffer);
					}
					else username += Random.Shared.Next(10).ToString();
				}

				_databaseContext.Users.Add(Domain.Models.Database.User.CreateNew(user.id, username));
				await _databaseContext.SaveChangesAsync();

				// get created user's id
				User newusr = await _databaseContext.Users.FirstAsync(u => u.DiscordUserId == user.id);
				Domain.Models.Session.Set(HttpContext, newusr.Id, newusr.Username);

				Response.Redirect("/settings");
			}
			else
			{
				Domain.Models.Session.Set(HttpContext, dbUser.Id, dbUser.Username);
				Response.Redirect("/");
			}
		}
	}

	record TokenResponse(string access_token, string token_type, int expires_in, string refresh_token, string scope);

	/*
	"{\"id\": \"158127066187825152\", \"username\": \"alexnj\", \"global_name\": \"alex\", \"avatar\": \"f6c61209645aa6eba134ef4be777ccf8\", \"discriminator\": \"0\", \"public_flags\": 128, \"flags\": 128, \"banner\": \"b283ac435381915cb15029fed93d8136\", \"banner_color\": \"#25bfd8\", \"accent_color\": 2473944, \"locale\": \"en-GB\", \"mfa_enabled\": true, \"premium_type\": 2, \"avatar_decoration\": null}"
	*/
	record UserResponse(string id, string username, string? global_name, string? avatar, string discriminator, int public_flags, int flags, string? banner, string? banner_color, int? accent_color, string locale, bool mfa_enabled, int premium_type, string? avatar_decoration);

}
