using System.Text.RegularExpressions;
using Ciri.Models.Modals.Moderation;
using Discord;
using Discord.Interactions;
using ShikimoriSharp;

namespace Ciri.Modules;

[Group("cinema", "cinema commands")]
[EnabledInDm(false)]
public class Cinema : InteractionModuleBase<SocketInteractionContext>
{
	private readonly Regex m_shikimoriRegex = new (@"(?:https?:\/\/)?shikimori\.(?:one|org)\/animes\/(?:[^0-9]*)(\d+)", RegexOptions.Compiled | RegexOptions.Singleline);
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
	[DefaultMemberPermissions(GuildPermission.Administrator)]
	public async Task ModalRequest(CinemaRequestModal modal)
	{
		var embed = new EmbedBuilder()
			.WithColor(3093046)
			.WithAuthor(modal.Name, url: modal.Url)
			.WithDescription($"Запросил: {Context.User.Mention}\n{modal.Description}");
		
		var match = m_shikimoriRegex.Match(modal.Url);
		if (match is { Success: true, Groups.Count: > 1 } && int.TryParse(match.Groups[1].Value, out var animeId))
		{
			var anime = await m_shikimoriClient.Animes.GetAnime(animeId);
			embed.WithAuthor(Context.User);
			embed.WithImageUrl("https://nyaa.shikimori.one" + anime.Image.Original);
			embed.WithUrl("https://shikimori.one" + anime.Url);
			embed.WithTitle($"{anime.Russian} / {anime.Name}");
			//TODO: add xml filtering, [char=12345] и [/char]
			embed.WithDescription(modal.Description);
		}
		
		await Context.Channel.SendMessageAsync(embed: embed.Build());
		await Context.Interaction.RespondAsync("Sent", ephemeral: true);
	}
}