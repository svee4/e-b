using Microsoft.EntityFrameworkCore;
using e_b.Domain.Models.Database;

namespace e_b.Domain.Models.Database;

public class DatabaseContext : DbContext
{
	public DbSet<User> Users => base.Set<User>();
	public DbSet<Content> Contents => base.Set<Content>();
	public DbSet<Folder> Folders => base.Set<Folder>();

	public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// User
		var User = modelBuilder.Entity<User>();
		User.HasIndex(u => u.DiscordUserId).IsUnique();
		User.HasIndex(u => u.Username).IsUnique();

		// Content
		var Content = modelBuilder.Entity<Content>();
		Content.HasOne<Folder>(c => c.Folder).WithMany().HasForeignKey("FolderId");
		Content.HasOne<User>(c => c.Owner).WithMany().HasForeignKey("OwnerId");
		Content.HasIndex(c => new { c.OwnerId, c.FolderId, c.Name }).IsUnique();

		// Folder
		var Folder = modelBuilder.Entity<Folder>();
		Folder.HasOne<User>(f => f.Owner).WithMany().HasForeignKey("OwnerId");
		Folder.HasIndex(f => new { f.OwnerId, f.Name }).IsUnique();
	}


	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));

}