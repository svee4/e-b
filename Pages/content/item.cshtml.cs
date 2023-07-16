using e_b.Domain.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Extensions;

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


				Domain.Models.View.OpenGraph og = new Domain.Models.View.OpenGraph();

				og.Title = item.Name;
				og.Description = item.Description;
				og.Url = Request.GetDisplayUrl();

				og.Type = item.Type switch
				{
					Domain.Models.Database.Content.ContentType.Video => "video.other",
					_ => "website"
				};

				string myBaseUrl = $"{Request.Scheme}://{Request.Host}";

				switch (item.Type)
				{
					case Domain.Models.Database.Content.ContentType.Image:
						og.Image = $"{myBaseUrl}/{ContentSource}";
						og.ImageAlt = item.Name;
						og.ImageWidth = item.Width;
						og.ImageHeight = item.Height;
						break;
					case Domain.Models.Database.Content.ContentType.Video:
						og.Extra.Add("video", $"{myBaseUrl}/{ContentSource}");
						og.Extra.Add("video:secure_url", $"{myBaseUrl}/{ContentSource}");
						og.Extra.Add("video:width", "1920"); // idk freeballin lmao
						og.Extra.Add("video:height", "1080");
						break;

				}


				ViewData["OpenGraph"] = og;
			}

			else mNotFound = true;
		}

	}
}
