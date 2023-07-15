using Microsoft.EntityFrameworkCore;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.secret.json", false);

builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
	options.Conventions.AddPageRoute("/content/item", "/content/{username}/{contentname}");
});

builder.Services.AddControllers();


// Add session state
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-7.0
builder.Services.AddDistributedMemoryCache();

// add database context
builder.Services.AddDbContext<e_b.Domain.Models.Database.DatabaseContext>(options =>
					options.UseNpgsql(builder.Configuration["DatabaseConnectionString"]));


builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// app.UseHsts(); // i dont know what hsts is so ill disable it
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();
app.MapControllers();

app.Run();
