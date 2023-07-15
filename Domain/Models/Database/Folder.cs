using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_b.Domain.Models.Database;

public class Folder
{
	public int Id { get; private set; } = 0;
	public User Owner { get; private set; } = null!;
	public int OwnerId { get; private set; } = 0;

	[Column(TypeName = "VARCHAR(64)")]
	public string Name { get; private set; } = "";
	public bool Unlisted { get; private set; } = false;
	public DateTime CreatedAt { get; private set; } = DateTime.MinValue;
}