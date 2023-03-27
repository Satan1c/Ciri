using Ciri.Modules.Utils;
using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ciri.Modules;

[Group("events", "events commands")]
[EnabledInDm(false)]
public class Events : InteractionModuleBase<SocketInteractionContext>
{
	[SlashCommand("send_event", "sends event")]
	public Task SendEvent(
		[Summary("time", "time of event beginning")]
		long time,
		[Summary("event_name", "name of event")]
		EventName eventName,
		[Summary("eventer", "user who will host the event")]
		IUser? eventer = null)
	{
		eventer ??= Context.User;
		var embed = eventName switch
		{
			EventName.Mafia => eventer.GetMafiaEmbed(time),
			EventName.Alias => eventer.GetAliasEmbed(time),
			EventName.WhoI => eventer.GetWhoIEmbed(time),
			EventName.Crocodile => eventer.GetCrocodileEmbed(time),
			EventName.Jackbox => eventer.GetJackboxEmbed(time),
			EventName.Tabletop => eventer.GetTabletopEmbed(time),
			EventName.Moments => eventer.GetMomentsEmbed(time),
			_ => throw new ArgumentOutOfRangeException(nameof(eventName), eventName, null)
		};
		
		return Context.SendEventEmbedAsync(embed);
	}
}

[JsonConverter(typeof(StringEnumConverter))]
public enum EventName
{
	Mafia,
	Alias,
	[ChoiceDisplay("Who I ?")]
	WhoI,
	Crocodile,
	Jackbox,
	Tabletop,
	Moments,
}