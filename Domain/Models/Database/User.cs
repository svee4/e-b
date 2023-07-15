using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_b.Domain.Models.Database;

public class User
{
	public int Id { get; private set; } = 0;

	[Column(TypeName = "CHAR(18)")]
	public string DiscordUserId { get; set; } = string.Empty;

	[Column(TypeName = "VARCHAR(32)")]
	public string Username { get; set; } = string.Empty;

	[Column(TypeName = "VARCHAR(512)")]
	public string? Description { get; set; } = null;

	[Column(TypeName = "VARCHAR(162)")] // 1 for /, 32 for username, 1 for /, 128 for content filename 
	public string? ProfilePicture { get; set; } = null;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public bool UsernameChanged { get; set; } = false;

	public static User CreateNew(string discordUserId, string username)
	{
		if (discordUserId.Length != 18 && discordUserId.Length != 17)
			throw new ArgumentException("Expected discord user id to be 18 or 17 characters long", nameof(discordUserId));

		if (username.Length > 32)
			throw new ArgumentException("Expected username to be equal to or less than 32 characters long", nameof(username));

		return new User
		{
			DiscordUserId = discordUserId,
			Username = username,
		};
	}


}