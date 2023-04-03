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
	private readonly DataBaseProvider m_dataBaseProvider;
	private readonly Category m_lcalCategory;
	private const ulong c_femaleRole = 691311950277115904;
	private const ulong c_maleRole = 691312169836347502;

	public Profile(DataBaseProvider dataBaseProvider, LocalizationManager localizationManager)
	{
		m_dataBaseProvider = dataBaseProvider;
		m_lcalCategory = localizationManager.GetCategory("profile");
	}

	[SlashCommand("info", "Shows user profile info")]
	public async Task Info([Summary("user", "user whose profile info to show")]IUser? user = null)
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
		[Summary("text", "new bio text"),
		 MinLength(0), MaxLength(127)]
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
		var profile = await m_dataBaseProvider.GetProfiles(user.Id);
		profile.Reputation += mode == RepMode.Add ? 1 : -1;
		
		await m_dataBaseProvider.SetProfiles(profile);
		await RespondAsync("Rep changed", ephemeral: true);
	}
	
	public enum RepMode
	{
		[ChoiceDisplay("add")]
		Add,
		[ChoiceDisplay("remove")]
		Remove
	}

	private EmbedBuilder GetEmbed(IGuildUser member, DataBase.Models.Profile profile)
	{
		var embed = GetEmbed((IUser) member, profile);
		var locale = m_lcalCategory.GetDataFor("member_profile");
		var data = locale.GetForLocale(Context);

		var title = data["title"].FormatWith(member);
		
		var rolesTitle = data["roles_title"];
		var rolesValue = data["roles_value"].FormatWith(new{member.RoleIds.Count});
		
		var joinedTitle = data["joined_title"];
		var joinedValue = data["joined_value"].FormatWith(new {JoinedAt = $"<t:{(member.JoinedAt?.ToUnixTimeSeconds() ?? 0).ToString()}:R>"});

		return embed
			.WithTitle(title)
			.WithThumbnailUrl(member.GetDisplayAvatar())
			.AddField(rolesTitle, rolesValue, true)
			.AddField(joinedTitle, joinedValue, true);
	}
	private EmbedBuilder GetEmbed(IUser user, DataBase.Models.Profile profile)
	{
		var locale = m_lcalCategory.GetDataFor("user_profile");
		var data = locale.GetForLocale(Context);
		
		var title = data["title"];
		var description =  data["description"].FormatWith(profile);
		
		var balanceTitle = data["balance_title"];
		var balanceValue = data["balance_value"].FormatWith(profile);
		
		var voiceTitle = data["voice_title"];
		var voiceValue = data["voice_value"].FormatWith(new {Voice = profile.Voice.ToLongTimeString()});
		
		var messagesTitle = data["messages_title"];
		var messagesValue = data["messages_value"].FormatWith(profile);
		
		var reputationTitle = data["reputation_title"];
		var reputationValue = data["reputation_value"].FormatWith(profile);
		
		var loverTitle = data["lover_title"];
		var loverValue = data[profile.Lover == 0 ? "lover_no_value" : "lover_value"].FormatWith(profile);

		return new EmbedBuilder()
			.WithThumbnailUrl(user.GetDisplayAvatarUrl())
			.WithTitle(title.FormatWith(user))
			.WithUrl($"https://discord.com/users/{user.Id.ToString()}")
			.WithDescription(description)
			.AddField(voiceTitle, voiceValue, true)
			.AddField(messagesTitle, messagesValue, true)
			.AddField(reputationTitle, reputationValue, true)
			.AddField(loverTitle, loverValue, true)
			.AddField(balanceTitle, balanceValue, true);
	}
}