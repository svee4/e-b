using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using e_b.Domain.Extensions;
using ViewModels = e_b.Domain.Models.View.search;

namespace e_b.Pages
{
	public class searchModel : PageModel
	{

		public List<ViewModels.ResultItem> Results = new List<ViewModels.ResultItem>();
		public ViewModels.SearchData SearchData = null!;

		private readonly IConfiguration _config;
		private readonly Domain.Models.Database.DatabaseContext _databaseContext;

		public searchModel(IConfiguration config, Domain.Models.Database.DatabaseContext databaseContext)
		{
			_config = config;
			_databaseContext = databaseContext;
		}


		public void OnGet([FromQuery] QueryModel query)
		{
			SearchData = new ViewModels.SearchData()
			{
				Name = query.Name ?? "",
				Username = query.Username ?? "",
				Type = query.Type ?? "",
				OrderBy = query.OrderBy ?? "",
				SortBy = query.SortBy ?? ""
			};

			bool sname = !string.IsNullOrWhiteSpace(query.Name),
				susername = !string.IsNullOrWhiteSpace(query.Username),
				stype = query.Type != "all";

			int type = !stype ? 0 : query.Type switch
			{
				"image" => (int)Domain.Models.Database.Content.ContentType.Image,
				"video" => (int)Domain.Models.Database.Content.ContentType.Video,
				"other" => -1,
				_ => 0
			};

			// theres like 10 million better ways to do this but i dont know them
			var build = _databaseContext.Contents.AsQueryable();
			if (sname)
				build = build.Where(c => c.Name.Contains(query.Name!));
			if (susername)
				build = build.Where(c => c.Owner.Username.Contains(query.Username!));
			if (type != 0)
			{
				if (type == -1)
					build = build.Where(c => c.Type != Domain.Models.Database.Content.ContentType.Image
											&& c.Type != Domain.Models.Database.Content.ContentType.Video);
				else
					build = build.Where(c => c.Type == (Domain.Models.Database.Content.ContentType)type);
			}

			switch (query.OrderBy)
			{
				case "date":
					build = query.SortBy == "asc" ? build.OrderBy(c => c.CreatedAt) : build.OrderByDescending(c => c.CreatedAt);
					break;
				case "name":
					build = query.SortBy == "asc" ? build.OrderBy(c => c.Name) : build.OrderByDescending(c => c.Name);
					break;
				case "username":
					build = query.SortBy == "asc" ? build.OrderBy(c => c.Owner.Username) : build.OrderByDescending(c => c.Owner.Username);
					break;
			}

			build = build.Where(c => !c.Unlisted).Include(c => c.Owner);

			string? baseDirectory = _config["VirtualContentDirectory"];
			if (baseDirectory is null)
				throw new Exception("VirtualContentDirectory not configured");

			Results = build.Take(100).Select(c => ViewModels.ResultItem.FromDatabase(c, baseDirectory)).ToList();

		}

		public class QueryModel
		{
			[BindProperty(Name = "n")]
			public string? Name { get; set; } = null;

			[BindProperty(Name = "u")]
			public string? Username { get; set; } = null;

			[BindProperty(Name = "t"), RegularExpression("^(all|image|video|other)$")]
			public string Type { get; set; } = "all";

			[BindProperty(Name = "ob"), RegularExpression("^(date|name|username)$")]
			public string OrderBy { get; set; } = "date";

			[BindProperty(Name = "sb"), RegularExpression("^(asc|desc)$")]
			public string SortBy { get; set; } = "desc";

		}

	}

}
