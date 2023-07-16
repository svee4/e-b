using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using e_b.Domain.Models.Api;
using e_b.Domain.Models.Database;
using e_b.Domain.Extensions;
using System.Text.RegularExpressions;

using e_b.Domain.Models.Api.UpdateSettings;

namespace e_b.Pages.api;

[ApiController]
[Route("api/updatesettings")]
public class UpdateSettings : ControllerBase
{

	private readonly IConfiguration _config;
	private readonly DatabaseContext _databaseContext;

	public UpdateSettings(IConfiguration config, DatabaseContext databaseContext)
	{
		_config = config;
		_databaseContext = databaseContext;
	}

	public IActionResult Post([FromForm] PostBody body)
	{

		var session = Domain.Models.Session.TryGet(HttpContext);
		if (session is null)
			return Unauthorized("Not authenticated");

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var user = _databaseContext.Users.Find(session.UserId);
		if (user is null)
			return Problem("Session user not found", statusCode: (int)HttpStatusCode.InternalServerError);

		bool anythingChanged = false;

		if (!body.Username.IsNullOrEmpty())
		{
			if (user.UsernameChanged)
			{
				ModelState.AddModelError("Username", "Username can only be changed once");
				return BadRequest(ModelState);
			}
			else if (body.Username == "c" || body.Username == "content")
			{
				// relics of old times
				ModelState.AddModelError("Username", "Username cannot be 'content'");
			}
			else if (_databaseContext.Users.Any(u => u.Username == body.Username))
			{
				ModelState.AddModelError("Username", "Username taken");
				return BadRequest(ModelState);
			}
			else
			{

				string[] forbiddenUsernames = { "api", "authenticate", "settings", "shared", "about", "error", "index", "search", "upload", "user",
												"admin", "content", "login", "logout", "register" };

				if (forbiddenUsernames.Contains(body.Username))
				{
					ModelState.AddModelError("Username", "Username is forbidden");
					return BadRequest(ModelState);
				}

				string oldname = user.Username;
				user.Username = body.Username!;
				user.UsernameChanged = true;
				anythingChanged = true;

				// need 2 update directory name... 
				string? baseDirectory = _config["PhysicalContentDirectory"];
				if (baseDirectory is null)
					return Problem("PhysicalContentDirectory not configured", statusCode: (int)HttpStatusCode.InternalServerError);

				string oldDirectory = Path.Combine(baseDirectory, oldname);
				string newDirectory = Path.Combine(baseDirectory, user.Username);
				if (Directory.Exists(oldDirectory))
					Directory.Move(oldDirectory, newDirectory);

				Domain.Models.Session.Set(HttpContext, user.Id, user.Username);



			}

		}

		if (!body.Description.IsNullOrEmpty())
		{
			user.Description = body.Description;
			anythingChanged = true;
		}

		if (!body.ProfilePicture.IsNullOrEmpty()) // like todo or something i guess lolz
		{

		}

		if (anythingChanged)
		{
			_databaseContext.SaveChanges();
			if (!ModelState.IsValid)
				return Ok(ModelState);

			return Ok(("message", "Changes saved"));
		}

		if (!ModelState.IsValid)
			return Ok(ModelState);


		return NoContent();

	}




}