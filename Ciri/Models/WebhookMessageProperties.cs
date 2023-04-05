using Discord;

namespace Ciri.Models;

public class WebhookMessageProperties : Discord.Webhook.WebhookMessageProperties
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

	public new IEnumerable<Embed>? Embeds
	{
		get => base.Embeds.IsSpecified ? base.Embeds.Value : null;
		set => base.Embeds = new Optional<IEnumerable<Embed>>(value!);
	}
}