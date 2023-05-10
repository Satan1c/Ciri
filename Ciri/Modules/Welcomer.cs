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
    public async Task RolseSelect(IRole[] values)
    {
        var ids = values.Select(x => x.Id);
        var user = Context.Guild.GetUser(Context.User.Id);
        
        await user.AddRolesAsync(values);
        await user.RemoveRolesAsync(MessagesConfig.Roles.Where(x => !ids.Contains(x)));
    }
}