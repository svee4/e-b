using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_b.Domain.Models.Database;


public class Content
{

	public enum ContentType
	{
		Unknown,
		Other,
		Image,
		Video,
	}


	public int Id { get; private set; } = 0;

	public User Owner { get; private set; } = null!;
	public int OwnerId { get; private set; } = 0;

	[Column(TypeName = "VARCHAR(128)")]
	public string Name { get; private set; } = "";

	[Column(TypeName = "VARCHAR(128)")]
	public string Filename { get; private set; } = "";

	[Column(TypeName = "VARCHAR(512)")]
	public string? Description { get; private set; } = "";

	public ContentType Type { get; private set; } = ContentType.Unknown;

	public int? Width { get; set; } = null;
	public int? Height { get; set; } = null;

	public Folder? Folder { get; private set; } = null;
	public int? FolderId { get; private set; } = null;

	public bool Unlisted { get; private set; } = false;
	public DateTime CreatedAt { get; private set; } = DateTime.MinValue;

	private Content() { }


	public static Result<Content, Exception> Create(User owner, string name, string filename, ContentType type, string? description, int? folderId, bool unlisted, int? width, int? height)
	{

		if (name.Length > 128)
			return new ArgumentOutOfRangeException(nameof(name)); // todo custom exception bc arg out of range is not meant for this i reckon

		if (filename.Length > 128)
			return new ArgumentOutOfRangeException(nameof(filename));

		if (description is not null && description.Length > 512)
			return new ArgumentOutOfRangeException(nameof(description));

		return new Content
		{
			Owner = owner,
			Name = name,
			Filename = filename,
			Type = type,
			Description = description,
			FolderId = folderId,
			Unlisted = unlisted,
			CreatedAt = DateTime.UtcNow,
			Width = width,
			Height = height,
		};

	}

}