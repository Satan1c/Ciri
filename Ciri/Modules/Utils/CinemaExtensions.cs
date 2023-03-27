using System.Text;
using System.Text.RegularExpressions;
using Discord;
using ShikimoriSharp.Classes;

namespace Ciri.Modules.Utils;

public static class CinemaExtensions
{
	private static readonly Regex s_characterRegex = new (@"\[(?:character=\w+|\/character)\]", RegexOptions.Compiled | RegexOptions.Singleline);

	public static EmbedBuilder GetAnimeEmbed(this AnimeID anime)
	{
		var url = $"https://shikimori.one{anime.Url}";
		var descriptionBuilder = new StringBuilder($"Status: `{anime.Status}`\n");
		
		if (anime is { Episodes: > 0, EpisodesAired: > 0 })
			descriptionBuilder.Append($"Episodes: `{anime.EpisodesAired.ToString()}{(anime.Status != "released" ? $"` / `{anime.Episodes.ToString()}" : "")}`\n");
		else 
			descriptionBuilder.Append("Episodes: `0`\n");
		
		if (anime.Genres != null)
			descriptionBuilder.Append($"\n `Genres`: `{string.Join("`, `", anime.Genres.Select(x => x.Name))}`");
		
		if (anime.Studios != null)
			descriptionBuilder.Append($"\n `Studios`: `{string.Join("`, `", anime.Studios.Select(x => x.Name))}`");
		
		if (anime.Score != null)
			descriptionBuilder.Append($"\n `Score`: `{anime.Score}`");
		
		if (anime.Franchise != null)
			descriptionBuilder.Append($"\n[Franchise]({url}/franchise)\n");
		
		descriptionBuilder.Append($"\n{anime.Description ?? anime.DescriptionSource ?? ""}");
		
		return new EmbedBuilder()
			.WithColor(new Color(43, 45, 49))
			.WithTitle($"{(string.IsNullOrEmpty(anime.Russian) ? "" : $"{anime.Russian} /")}{anime.Name}")
			.WithUrl(url)
			.WithImageUrl($"https://shikimori.one{anime.Image?.Original}")
			.WithDescription(descriptionBuilder.ClearDescription());
	}
	
	private static string ClearDescription(this StringBuilder descriptionBuilder)
	{
		return descriptionBuilder.ToString().ClearDescription();
	}
	private static string ClearDescription(this string description)
	{
		return s_characterRegex.Replace(description, "");
	}
}