using Discord;

namespace Ciri.Models;

public class MessageProperties : Discord.MessageProperties
{
	public new string? Content
	{
		get => base.Content.IsSpecified ? base.Content.Value : null;
		set => base.Content = new Optional<string>(value!);
	}
	
	public new MessageComponent? Components
	{
		get => base.Components.IsSpecified ? base.Components.Value : null;
		set => base.Components = new Optional<MessageComponent>(value!);
	}

	public new Embed? Embed
	{
		get => base.Embed.IsSpecified ? base.Embed.Value : null;
		set => base.Embed = new Optional<Embed>(value!);
	}

	public new Embed[]? Embeds
	{
		get => base.Embeds.IsSpecified ? base.Embeds.Value : null;
		set => base.Embeds = new Optional<Embed[]>(value!);
	}
}