using Ciri.Models;
using Ciri.Modules.Configs;
using Ciri.Modules.Utils;
using DataBase;
using Discord;
using Discord.WebSocket;
using static Ciri.Modules.Configs.ImagesConfig;

namespace Ciri.Handlers;

public class GuildEvents
{
	public static IVoiceChannel MembersCount = null!;

	private static readonly Embed s_bumpEmbed = new EmbedBuilder()
		.WithTitle("Бамп")
		.WithDescription($"Вы полуили **__50__**{EmojiConfig.HeartVal}")
		.WithColor(3093046)
		.Build();

	private readonly DiscordSocketClient m_client;
	private readonly DataBaseProvider m_dataBaseProvider;
	private ITextChannel m_boostChannel;

	private ITextChannel m_logChannel;
	public IReadOnlyDictionary<ulong, ProfitData> Profit;

	public GuildEvents(DiscordSocketClient client, DataBaseProvider dataBaseProvider)
	{
		m_client = client;
		m_dataBaseProvider = dataBaseProvider;

		m_client.UserJoined += OnMemberJoined;
		m_client.UserLeft += OnMemberLeft;
		m_client.MessageReceived += OnMessageReceived;
		m_client.MessageUpdated += OnMessageEdit;
		//m_client.MessageReceived += OnMessageCreate;
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
			if (member.RoleIds.Contains<ulong>(542017661341794304))
				admins.AddLast(member.Id);
			else if (member.RoleIds.Contains<ulong>(698496217671401482))
				dev.AddLast(member.Id);
			else if (member.RoleIds.Contains<ulong>(542017417837281280))
				moder.AddLast(member.Id);
			else if (member.RoleIds.Contains<ulong>(803921489443029002))
				preModer.AddLast(member.Id);
			else if (member.RoleIds.Contains<ulong>(686895612020654108))
				eventer.AddLast(member.Id);
			else if (member.RoleIds.Contains<ulong>(639707864591630337))
				helper.AddLast(member.Id);
			else if (member.RoleIds.Contains<ulong>(1091762902945505301))
				closeMod.AddLast(member.Id);
			else if (member.RoleIds.Contains<ulong>(789431764514504724))
				prManager.AddLast(member.Id);

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
			MembersCount = membersCount;

		await MembersCount.ModifyAsync(x =>
			x.Name = $"🌹: {m_client.GetGuild(542005378049638400).MemberCount.ToString()}");
	}

	public async Task OnMessageEdit(Cacheable<IMessage, ulong> с, SocketMessage message, ISocketMessageChannel channel)
	{
		if (!message.Author.IsBot || message.Author.Id != 464272403766444044) return;

		var title = message.Embeds.First().Description?.Trim();
		if (string.IsNullOrEmpty(title) || !title.StartsWith("**Успешный Up!**")) return;

		var reference = (await channel.GetMessageAsync(message.Reference.MessageId.Value))!;
		var profile = await m_dataBaseProvider.GetProfiles(reference.Author.Id);

		profile.Hearts += 50;

		await m_dataBaseProvider.SetProfiles(profile);
		await channel.SendMessageAsync(embed: s_bumpEmbed, messageReference: message.Reference,
			allowedMentions: AllowedMentions.None);
	}

	/*public async Task OnMessageCreate(SocketMessage message)
	{
		if (!message.Author.IsBot) return;
	}*/

	public async Task OnMemberJoined(SocketGuildUser member)
	{
		await MembersCount.ModifyAsync(x => x.Name = $"🌹: {member.Guild.MemberCount.ToString()}");
		await m_logChannel.SendMessageAsync(embed: member.GetWelcomeEmbed());
		//newcomer role
		await member.AddRoleAsync(542012055775870976);
		await ClientEvents.OnLog(new LogMessage(LogSeverity.Info, nameof(OnMemberJoined),
			$"User {member.Username} joined the guild"));
	}

	public async Task OnMemberLeft(SocketGuild guild, SocketUser user)
	{
		await MembersCount.ModifyAsync(x => x.Name = $"🌹: {guild.MemberCount.ToString()}");
		await m_logChannel.SendMessageAsync(embed: user.GetGoodbyeEmbed());
		await ClientEvents.OnLog(new LogMessage(
			LogSeverity.Info,
			nameof(OnMemberLeft),
			$"User {user.Username} left from the guild"));
	}

	public async Task OnMessageReceived(SocketMessage message)
	{
		if (message is not
			    { Channel: ITextChannel { Id: 718427495640203264 }, Type: MessageType.UserPremiumGuildSubscription }
		    || message.Author is not SocketGuildUser member)
			return;
		
		await ClientEvents.OnLog(new LogMessage(
			LogSeverity.Info,
			nameof(OnMessageReceived),
			$"User {member.Username} boosted the guild"));

		await m_boostChannel.SendMessageAsync(embed: new EmbedBuilder()
			.WithTitle($"{member.Username}\nЗабустил сервер!")
			.WithDescription($"<@&{709738102394191984}>\nОгромное спасибо, что помогаете серверу!!")
			.WithThumbnailUrl(member.GetDisplayAvatarUrl() ?? member.GetDefaultAvatarUrl())
			.WithImageUrl(BoostGif)
			.WithColor(3093046)
			.Build());
	}
}