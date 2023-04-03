using Ciri.Models.Modals.Moderation;
using Discord;
using Discord.Interactions;

namespace Ciri.Modules;

/*[Group("moderation", "moderation commands")]
[EnabledInDm(false)]
public class Moderation : InteractionModuleBase<SocketInteractionContext>
{
	[SlashCommand("ban", "ban command")]
	[DefaultMemberPermissions(GuildPermission.Administrator)]
	public Task Ban(IUser user, string reason)
	{
		//return Context.Guild.AddBanAsync(user, reason: reason);
		return Context.Interaction.RespondAsync($"{user.Id} {user.Username} {reason}", ephemeral: true);
	}
	
	[UserCommand("Ban")]
	[DefaultMemberPermissions(GuildPermission.Administrator)]
	public Task BanContext(IUser user)
	{
		return Context.Interaction.RespondWithModalAsync<BanModal>($"ban_context_{user.Id}");
	}
	
	[ModalInteraction("ban_context_*", true)]
	[DefaultMemberPermissions(GuildPermission.Administrator)]
	public Task ModalBan(IUser user, BanModal modal)
	{
		return Ban(user, modal.Reason);
	}
}*/