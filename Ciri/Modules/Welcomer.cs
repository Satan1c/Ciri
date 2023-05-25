using Ciri.Modules.Configs.Welcomer;
using Discord;
using Discord.Interactions;

namespace Ciri.Modules;

[Group("welcomer", "welcomer interactions")]
public class Welcomer : InteractionModuleBase<SocketInteractionContext>
{
	[SlashCommand("send_message", "will send message with categories")]
	public async Task WelcomeMessage()
	{
		var props = MessagesConfig.WelcomeProperties;
		await RespondAsync(embeds: props.Embeds, components: props.Components);
	}

	[ComponentInteraction("welcome_category", true)]
	public async Task WelcomeSelect(string[] values)
	{
		var value = values[0];
		var props = value switch
		{
			"roles" => MessagesConfig.RolesProperties,
			"map" => MessagesConfig.MapProperties,
			"bots" => MessagesConfig.BotsProperties,
			"donate" => MessagesConfig.DonateProperties
		};

		await RespondAsync(ephemeral: true, embeds: props.Embeds, components: props.Components);
	}

	[ComponentInteraction("roles_select", true)]
	public async Task RolesSelect(string[] rawValues)
	{
		var values = rawValues.Select(x => Context.Guild.GetRole(ulong.Parse(x)) as IRole).ToArray();
		
		var user = Context.Guild.GetUser(Context.User.Id);

		var add = values.Where(x => !user.Roles.Contains(x)).Select(x => x.Id).ToArray();
		var delete = MessagesConfig.Roles.Where(x => !add.Contains(x)).ToArray();

		await user.AddRolesAsync(add);
		await user.RemoveRolesAsync(delete);

		await RespondAsync("Done", ephemeral: true);
	}
}