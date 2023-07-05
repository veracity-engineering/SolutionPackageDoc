using DNV.Web.Swagger;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Swagger.Demo.Controllers;

[Route("[controller]")]
public class TMDBController : ApiControllerBase
{
	private static readonly SampleData SampleData;

	static TMDBController()
	{
		var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
		jsonOptions.Converters.Add(new JsonStringEnumConverter());

		var assembly = typeof(TMDBController).Assembly;
		using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.SampleData.json");
		SampleData = JsonSerializer.Deserialize<SampleData>(stream!, jsonOptions)!;
	}

	public TMDBController()
	{
	}

	[HttpGet("movies")]
	[SwaggerTags("Movie")]
	public IEnumerable<Video> GetMovies() => SampleData.Movies;

	[HttpGet("movies/{id:guid}")]
	[SwaggerTags("Movie")]
	public Video? GetMovie(Guid id) => SampleData.Movies.FirstOrDefault(m => m.Id == id);

	[HttpGet("tvshows")]
	[SwaggerTags("TVShow")]
	public IEnumerable<Video> GetTVShows() => SampleData.Movies;

	[HttpGet("tvshows/{id:guid}")]
	[SwaggerTags("TVShow")]
	public Video? GetTVShow(Guid id) => SampleData.Movies.FirstOrDefault(m => m.Id == id);
}

public record SampleData(IEnumerable<Video> Movies);

public record Video(Guid Id, string Name, DateTime ReleaseDate, int Rating, VideoType type);

public enum VideoType
{
	Movie,
	TVShow
}
