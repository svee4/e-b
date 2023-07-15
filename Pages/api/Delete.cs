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
[Route("api/delete")]
public class Delete : ControllerBase
{

	private readonly IConfiguration _config;
	private readonly DatabaseContext _databaseContext;

	public Delete(IConfiguration config, DatabaseContext databaseContext)
	{
		_config = config;
		_databaseContext = databaseContext;
	}

	[HttpDelete]
	public IActionResult Handle([FromQuery] string name)
	{

		var session = Domain.Models.Session.TryGet(HttpContext);
		if (session is null)
			return Unauthorized("Not authenticated");

		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var user = _databaseContext.Users.Find(session.UserId);
		if (user is null)
			return Problem("Session user not found", statusCode: (int)HttpStatusCode.InternalServerError);

		var content = _databaseContext.Contents.FirstOrDefault(c => c.Name == name && c.OwnerId == user.Id);
		if (content is null)
			return NotFound("Content not found");

		_databaseContext.Remove(content);
		_databaseContext.SaveChanges();

		// delete physical file
		string? baseDirectory = _config["PhysicalContentDirectory"];
		if (baseDirectory is null)
			return Problem("PhysicalContentDirectory not configured", statusCode: (int)HttpStatusCode.InternalServerError);

		string directory = Path.Combine(baseDirectory, user.Username);
		string path = Path.Combine(directory, content.Filename);

		System.IO.File.Delete(path);

		return Ok();

	}




}