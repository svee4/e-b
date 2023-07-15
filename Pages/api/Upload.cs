using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using e_b.Domain.Models.Api.Upload;
using System.Net;
using e_b.Domain.Models.Api;
using e_b.Domain.Models.Database;
using e_b.Domain.Extensions;
using System.Text.RegularExpressions;

namespace e_b.Pages.api;

[ApiController]
[Route("api/upload")]
public class Upload : ControllerBase
{

	private readonly IConfiguration _config;
	private readonly DatabaseContext _databaseContext;

	public Upload(IConfiguration config, DatabaseContext databaseContext)
	{
		_config = config;
		_databaseContext = databaseContext;
	}

	[HttpPost]
	[Consumes("multipart/form-data")]
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


		bool isDuplicate = _databaseContext.Contents.Any(c => c.Name == body.Name && c.OwnerId == user.Id);
		if (isDuplicate)
			return BadRequest("Duplicate name");

		// max 100 files for now
		if (_databaseContext.Contents.Count(c => c.OwnerId == user.Id) >= 100)
			return BadRequest("Users are limited to 100 files, plz contact admin if you actually reached this its just a temporarry thing :p");

		var content = Domain.Models.Database.Content.Create(user, body.Name, body.Filename, body.Type, body.Description, null, body.Unlisted);
		if (content.IsErr)
			return BadRequest($"{content.Error!.GetType()}: {content.Error!.Message}"); // im LAZY


		_databaseContext.Add(content.Value!);
		_databaseContext.SaveChanges();

		// create physical file
		string? baseDirectory = _config["PhysicalContentDirectory"];
		if (baseDirectory is null)
			return Problem("PhysicalContentDirectory not configured", statusCode: (int)HttpStatusCode.InternalServerError);

		string directory = Path.Combine(baseDirectory, user.Username);

		Directory.CreateDirectory(directory);
		// save file
		using (var fileStream = new FileStream(Path.Combine(directory, body.Filename), FileMode.Create))
			body.File.CopyTo(fileStream);


		return Created($"/{user.Username}/{content.Value!.Name}", new
		{
			message = "ok",
			username = user.Username,
			contentName = body.Name
		});
	}




}