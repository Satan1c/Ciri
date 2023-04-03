using Ciri.Models;
using Ciri.Modules.Utils;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Ciri.Handlers;

public class GuildEvents
{
	private readonly DiscordSocketClient m_client;
	
	public static IVoiceChannel m_membersCount = null;
	public IReadOnlyDictionary<ulong, ProfitData> Profit;
	
	private ITextChannel m_logChannel = null;
	private ITextChannel m_boostChannel = null;
	private CronTimer m_timer = new("* * * * *", "UTC");

	public GuildEvents(DiscordSocketClient client)
	{
		m_client = client;
		
		m_client.UserJoined += OnMemberJoined;
		m_client.UserLeft += OnMemberLeft;
		m_client.MessageReceived += OnMessageReceived;
	}

	public async Task Init()
	{
		if (await m_client.Rest.GetGuildAsync(542005378049638400, true) is not IGuild guild)
			return;
		
		var admins = new LinkedList<ulong>();
		var dev = new LinkedList<ulong>();
		var moder = new LinkedList<ulong>();
		var preModer = new LinkedList<ulong>();
		var eventer = new LinkedList<ulong>();
		var helper = new LinkedList<ulong>();
		var closeMod = new LinkedList<ulong>();
		var prManager = new LinkedList<ulong>();
		foreach (var member in await guild.GetUsersAsync())
		{
			if (member.RoleIds.Contains<ulong>(542017661341794304))
			{
				admins.AddLast(member.Id);
			}
			else if (member.RoleIds.Contains<ulong>(698496217671401482))
			{
				dev.AddLast(member.Id);
			}
			else if (member.RoleIds.Contains<ulong>(542017417837281280))
			{
				moder.AddLast(member.Id);
			}
			else if (member.RoleIds.Contains<ulong>(803921489443029002))
			{
				preModer.AddLast(member.Id);
			}
			else if (member.RoleIds.Contains<ulong>(686895612020654108))
			{
				eventer.AddLast(member.Id);
			}
			else if (member.RoleIds.Contains<ulong>(639707864591630337))
			{
				helper.AddLast(member.Id);
			}
			else if (member.RoleIds.Contains<ulong>(1091762902945505301))
			{
				closeMod.AddLast(member.Id);
			}
			else if (member.RoleIds.Contains<ulong>(789431764514504724))
			{
				prManager.AddLast(member.Id);
			}
		}
		
		Profit = new Dictionary<ulong, ProfitData>
		{
			{ 542017661341794304, new ProfitData(300, admins) },
			{ 698496217671401482, new ProfitData(300, dev) },
			{ 542017417837281280, new ProfitData(200, moder) },
			{ 803921489443029002, new ProfitData(150, preModer) },
			{ 686895612020654108, new ProfitData(300, eventer) },
			{ 639707864591630337, new ProfitData(50, helper) },
			{ 1091762902945505301, new ProfitData(200, closeMod) },
			{ 789431764514504724, new ProfitData(50, prManager) }
		}.AsReadOnly();
		
		var channel = await guild.GetChannelAsync(639709192042709002) as IChannel;
		
		if (channel is ITextChannel logChannel)
			m_logChannel = logChannel;
		
		channel = await guild.GetChannelAsync(684011135287951392);
		
		if (channel is ITextChannel boostChannel)
			m_boostChannel = boostChannel;
		
		channel = await guild.GetChannelAsync(714345605467471914);
		
		if (channel is IVoiceChannel membersCount)
			m_membersCount = membersCount;

		await m_membersCount.ModifyAsync(x => x.Name = $"🌹: {m_client.GetGuild(542005378049638400).MemberCount.ToString()}");

		channel = await guild.GetChannelAsync(770669968329146378);
		if (channel is IVoiceChannel timeChannel)
		{
			m_timer.OnOccurence += async (_, _) =>
			{
				await timeChannel.ModifyAsync(x => x.Name = $"🕒 {DateTime.UtcNow.AddHours(3).ToString("HH:mm")}");
			};
			m_timer.Start();
		}
	}

	public async Task OnMemberJoined(SocketGuildUser member)
	{
		await m_membersCount.ModifyAsync(x => x.Name = $"🌹: {member.Guild.MemberCount.ToString()}");
		await m_logChannel.SendMessageAsync(embed: member.GetWelcomeEmbed());
		//newcomer role
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
		if (message is not { Channel: ITextChannel { Id: 718427495640203264 }, Type: MessageType.UserPremiumGuildSubscription }
		    || message.Author is not SocketGuildUser member)
			return;
		//booster role add
		await member.AddRoleAsync(812189549295566889);
		await ClientEvents.OnLog(new LogMessage(
			LogSeverity.Info,
			nameof(OnMessageReceived),
			$"User {member.Username} boosted the guild"));
		
		await m_boostChannel.SendMessageAsync(embed: new EmbedBuilder()
			.WithTitle($"{member.Username}\nЗабустил сервер!")
			.WithDescription($"<@&{812189549295566889}>\nОгромное спасибо, что помогаете серверу!!")
			.WithColor(3093046)
			.Build());
	}
}