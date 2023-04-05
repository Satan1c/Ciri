using System.Text.RegularExpressions;
using Ciri.Models.Modals.Moderation;
using Ciri.Modules.Utils;
using Discord;
using Discord.Interactions;
using ShikimoriSharp;

namespace Ciri.Modules;

[Group("cinema", "cinema commands")]
[EnabledInDm(false)]
public class Cinema : InteractionModuleBase<SocketInteractionContext>
{
	private static readonly Regex s_shikimoriRegex =
		new(@"(?:https?:\/\/)?shikimori\.(?:one|org)\/animes\/(?:[^0-9]*)(\d+)",
			RegexOptions.Compiled | RegexOptions.Singleline);

	private readonly ShikimoriClient m_shikimoriClient;

	public Cinema(ShikimoriClient shikimoriClient)
	{
		m_shikimoriClient = shikimoriClient;
	}

	[SlashCommand("send_request", "sends request to watch")]
	public Task SendRequest()
	{
		return Context.Interaction.RespondWithModalAsync<CinemaRequestModal>("cinema_request");
	}

	[ModalInteraction("cinema_request", true)]
	public async Task ModalRequest(CinemaRequestModal modal)
	{
		var embed = new EmbedBuilder()
			.WithColor(3093046)
			.WithAuthor(modal.Name, url: modal.Url)
			.WithDescription($"Запросил: {Context.User.Mention}\n{modal.Description}")!;

		var match = s_shikimoriRegex.Match(modal.Url);
		if (match is { Success: true, Groups.Count: > 1 } && int.TryParse(match.Groups[1].Value, out var animeId))
		{
			var anime = await m_shikimoriClient.Animes.GetAnime(animeId);
			embed = anime.GetAnimeEmbed().WithAuthor(Context.User)!;
			embed.WithDescription(modal.Description);
		}

		await Context.Channel.SendMessageAsync(embed: embed.Build());
		await RespondAsync("Sent", ephemeral: true);
	}
}