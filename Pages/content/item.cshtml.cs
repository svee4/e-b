using e_b.Domain.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace e_b.Pages.content
{
	public class itemModel : PageModel
	{

		public string ContentSource;
		public string Username;
		public string ContentName;
		public string? Description;
		public string ContentFilename;
		public Domain.Models.Database.Content.ContentType ContentType;

		public bool mNotFound = false;

		private readonly IConfiguration _config;
		private readonly DatabaseContext _context;

		public itemModel(IConfiguration config, DatabaseContext context)
		{
			_config = config;
			_context = context;
		}


		public async Task OnGet(string username, string contentname)
		{
			Username = username;
			ContentName = contentname;

			Domain.Models.Database.Content? item = await _context.Contents.FirstOrDefaultAsync(x => x.Name == contentname);
			if (item is not null)
			{
				string? baseDirectory = _config["VirtualContentDirectory"];
				if (baseDirectory is null)
					throw new Exception("VirtualContentDirectory not configured");

				ContentSource = "/" + Path.Combine(baseDirectory, username, item.Filename);
				ContentFilename = item.Filename;
				Description = item.Description;
				ContentType = item.Type;
			}
			else mNotFound = true;
		}
	}
}
