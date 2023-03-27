using Ciri.Modules.Utils;
using Discord;
using Discord.WebSocket;

namespace Ciri.Handlers;

public class GuildEvents
{
	private readonly DiscordSocketClient m_client;
	
	public static IVoiceChannel m_membersCount = null;
	private ITextChannel m_logChannel = null;
	private ITextChannel m_boostChannel = null;
	private IRole m_boostRole = null;

	public GuildEvents(DiscordSocketClient client)
	{
		m_client = client;
		
		m_client.UserJoined += OnMemberJoined;
		m_client.UserLeft += OnMemberLeft;
		m_client.MessageReceived += OnMessageReceived;
	}

	public async Task Init()
	{
		var channel = await m_client.Rest.GetChannelAsync(639709192042709002) as IChannel;
		
		if (channel is ITextChannel logChannel)
			m_logChannel = logChannel;
		
		channel = await m_client.Rest.GetChannelAsync(684011135287951392);
		
		if (channel is ITextChannel boostChannel)
			m_boostChannel = boostChannel;
		
		channel = await m_client.Rest.GetChannelAsync(714345605467471914);
		
		if (channel is IVoiceChannel membersCount)
			m_membersCount = membersCount;

		if (await m_client.Rest.GetGuildAsync(542005378049638400, true) is not IGuild guild)
			return;
		
		await m_membersCount.ModifyAsync(x => x.Name = $"🌹: {m_client.GetGuild(guild.Id).MemberCount.ToString()}");
		
		var role = guild.GetRole(812189549295566889);
		if (role is not null)
			m_boostRole = role;
	}

	public async Task OnMemberJoined(SocketGuildUser member)
	{
		await m_membersCount.ModifyAsync(x => x.Name = $"🌹: {member.Guild.MemberCount.ToString()}");
		await m_logChannel.SendMessageAsync(embed: member.GetWelcomeEmbed());
		await member.AddRoleAsync(542012055775870976);
		await ClientEvents.OnLog(new LogMessage(LogSeverity.Info, nameof(OnMemberJoined), $"User {member.Username} joined the guild"));
	}
	
	public async Task OnMemberLeft(SocketGuild guild, SocketUser user)
	{
		await m_membersCount.ModifyAsync(x => x.Name = $"🌹: {guild.MemberCount.ToString()}");
		await m_logChannel.SendMessageAsync(embed: user.GetGoodbyeEmbed());
		await ClientEvents.OnLog(new LogMessage(
			LogSeverity.Info,
			nameof(OnMemberLeft),
			$"User {user.Username} left from the guild"));
	}

	public async Task OnMessageReceived(SocketMessage message)
	{
		if ((message is not { Channel: ITextChannel { Id: 718427495640203264 }, Type: MessageType.UserPremiumGuildSubscription })
		    || (message.Author is not SocketGuildUser member))
			return;
		
		await member.AddRoleAsync(542005378049638400);
		await ClientEvents.OnLog(new LogMessage(
			LogSeverity.Info,
			nameof(OnMessageReceived),
			$"User {member.Username} boosted the guild"));

		await m_boostChannel.SendMessageAsync(embed: new EmbedBuilder()
			.WithTitle($"{member.Username}\nЗабустил сервер!")
			.WithDescription($"{m_boostRole.Mention}\nОгромное спасибо, что помогаете серверу!!")
			.WithColor(3093046)
			.Build());
	}
}