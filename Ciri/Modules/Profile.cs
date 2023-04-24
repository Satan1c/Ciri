using Ciri.Modules.Utils;
using DataBase;
using Discord;
using Discord.Interactions;
using Localization;
using Localization.Models;

namespace Ciri.Modules;

[Group("profile", "Profile commands")]
[EnabledInDm(false)]
public class Profile : InteractionModuleBase<SocketInteractionContext>
{
	public enum RepMode
	{
		[ChoiceDisplay("add")] Add,
		[ChoiceDisplay("remove")] Remove
	}

	private readonly DataBaseProvider m_dataBaseProvider;
	private readonly Category m_localeCategory;

	public Profile(DataBaseProvider dataBaseProvider, LocalizationManager localizationManager)
	{
		m_dataBaseProvider = dataBaseProvider;
		m_localeCategory = localizationManager.GetCategory("profile");
	}

	[SlashCommand("info", "Shows user profile info")]
	public async Task Info([Summary("user", "user whose profile info to show")] IUser? user = null)
	{
		user ??= Context.User;
		var profile = await m_dataBaseProvider.GetProfiles(user.Id);
		var member = Context.Guild.GetUser(user.Id);
		if (member != null)
		{
			await RespondAsync(embed: GetEmbed(member, profile).Build());
			return;
		}

		await RespondAsync(embed: GetEmbed(user, profile).Build());
	}

	[SlashCommand("about_me", "change your profile bio")]
	public async Task AboutMe(
		[Summary("text", "new bio text")] [MinLength(0)] [MaxLength(127)]
		string text)
	{
		var profile = await m_dataBaseProvider.GetProfiles(Context.User.Id);
		profile.Bio = text;

		await m_dataBaseProvider.SetProfiles(profile);
		await RespondAsync("Profile bio changed", ephemeral: true);
	}

	[SlashCommand("lover", "change your lover")]
	public async Task Lover(IUser? user = null)
	{
		var profile = await m_dataBaseProvider.GetProfiles(Context.User.Id);
		profile.Lover += user?.Id ?? null;

		await m_dataBaseProvider.SetProfiles(profile);
		await RespondAsync("Profile lover changed", ephemeral: true);
	}

	[SlashCommand("rep", "give reputation to user")]
	public async Task Rep(IUser user, RepMode mode = RepMode.Add)
	{
		var author = await m_dataBaseProvider.GetProfiles(Context.User.Id);
		var profile = await m_dataBaseProvider.GetProfiles(user.Id);

		switch (mode)
		{
			case RepMode.Add:
				if (author.RepGiven.Contains(user.Id))
				{
					await RespondAsync("You already gave rep to this user", ephemeral: true);
					return;
				}
				
				profile.Reputation++;
				author.RepGiven.AddLast(user.Id);
				break;
			
			case RepMode.Remove:
				if (!author.RepGiven.Contains(user.Id))
				{
					await RespondAsync("You didn't give rep to this user", ephemeral: true);
					return;
				}
				
				profile.Reputation--;
				author.RepGiven.Remove(user.Id);
				break;
		}
		
		await m_dataBaseProvider.SetProfiles(new[] { profile, author });
		await RespondAsync("Rep changed", ephemeral: true);
	}

	private EmbedBuilder GetEmbed(IGuildUser member, DataBase.Models.Profile profile)
	{
		var embed = GetEmbed((IUser)member, profile);
		var locale = m_localeCategory.GetDataFor("member_profile");
		var data = locale.GetForLocale(Context);

		var title = data["title"].FormatWith(member);

		var rolesTitle = data["roles_title"];
		var rolesValue = data["roles_value"].FormatWith(new { member.RoleIds.Count });

		var joinedTitle = data["joined_title"];
		var joinedValue = data["joined_value"].FormatWith(new
			{ JoinedAt = (member.JoinedAt?.ToUnixTimeSeconds() ?? 0).ToString() });

		return embed
			.WithTitle(title)
			.WithThumbnailUrl(member.GetDisplayAvatar())
			.AddField(rolesTitle, rolesValue, true)
			.AddField(joinedTitle, joinedValue, true);
	}

	private EmbedBuilder GetEmbed(IUser user, DataBase.Models.Profile profile)
	{
		var locale = m_localeCategory.GetDataFor("user_profile");
		var data = locale.GetForLocale(Context);

		var title = data["title"].FormatWith(user);
		var description = data["description"].FormatWith(profile);

		var balanceTitle = data["balance_title"];
		var balanceValue = data["balance_value"].FormatWith(profile);

		var voiceTitle = data["voice_title"];
		var voiceValue = data["voice_value"].FormatWith(new { Voice = profile.Voice.ToLongTimeString() });

		var messagesTitle = data["messages_title"];
		var messagesValue = data["messages_value"].FormatWith(profile);

		var reputationTitle = data["reputation_title"];
		var reputationValue = data["reputation_value"].FormatWith(profile);

		var loverTitle = data["lover_title"];
		var loverValue = data[profile.Lover is null or 0 ? "lover_no_value" : "lover_value"].FormatWith(profile);

		return new EmbedBuilder()
			.WithThumbnailUrl(user.GetDisplayAvatarUrl())
			.WithTitle(title)
			.WithUrl($"https://discord.com/users/{user.Id.ToString()}")
			.WithDescription(description)
			.AddField(voiceTitle, voiceValue, true)
			.AddField(messagesTitle, messagesValue, true)
			.AddField(reputationTitle, reputationValue, true)
			.AddField(loverTitle, loverValue, true)
			.AddField(balanceTitle, balanceValue, true);
	}
}