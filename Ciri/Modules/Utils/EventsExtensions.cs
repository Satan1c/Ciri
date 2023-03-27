using Discord;
using static Ciri.Modules.Configs.EmojiConfig;
using static Ciri.Modules.Configs.ImagesConfig;
using static Ciri.Modules.Utils.Extensions;

namespace Ciri.Modules.Utils;

public static class EventsExtensions
{
	private const string c_eventChannel = "https://discord.com/channels/542005378049638400/700739189477474334";
	
	public static async Task SendEventEmbedAsync(this IInteractionContext context, Embed embed)
	{
		var message = (await context.Channel.SendMessageAsync(
			"<@&700009031879622767>",
			embed: embed,
			components: new ComponentBuilder()
				.WithButton("Голосовой канал", style: ButtonStyle.Link, url: "https://discord.gg/tC6eFDr")
				.Build()))!;
		await context.Interaction.RespondAsync("Sent event", ephemeral: true);
		
		if (message.Channel is ITextChannel channel)
			await channel.CreateThreadAsync(
				"Обсуждение",
				channel.GetChannelType() == ChannelType.News ? ThreadType.NewsThread : ThreadType.PublicThread,
				ThreadArchiveDuration.OneDay,
				message);
	}
	
	private static string GetTitle(string name, long time)
	{
		return $"{name} | <t:{time}:t>";
	}
	private static string GetDescription(string name, string description, ulong eventer)
	{
		return $"{Star}**- Ведущий:** <@{eventer}>\n\n*Приглашаем тебя сыграть в [{name}]({c_eventChannel})*.\n{description}";
	}

	public static Embed GetMafiaEmbed(this IUser user, long time)
	{
		return new EmbedBuilder()
			.WithTitle(GetTitle("Мафия", time))
			.WithDescription(GetDescription(
				"Мафию",
				"Салонная командная психологическая пошаговая ролевая игра " +
				"с детективным сюжетом, моделирующая борьбу информированных друг о друге " +
				"членов организованного меньшинства с неорганизованным большинством.",
				user.Id))
			.WithColor(3093046)
			.AddPrizes(MafiaImage, new []
			{
				new Prize("мафия", 100),
				new Prize("мирные", 50),
				new Prize("участие", 20)
			})
			.Build();
	}

	public static Embed GetAliasEmbed(this IUser user, long time)
	{
		return new EmbedBuilder()
			.WithTitle(GetTitle("Шляпа", time))
			.WithDescription(GetDescription(
				"Шляпу",
				"Игроки делятся на команды по 2 человека в каждой. " +
				"Ведущий отправляет одному из 2-х игроков слова, которые тот должен объяснить своему партнёру, " +
				"не используя однокоренные и созвучные слова",
				user.Id))
			.WithColor(3093046)
			.AddPrizes(AliasImage)
			.Build();
	}

	public static Embed GetWhoIEmbed(this IUser user, long time)
	{
		return new EmbedBuilder()
			.WithTitle(GetTitle("Кто я?", time))
			.WithDescription(GetDescription("Кто я?",
				"Вы играете каждый сам за себя, вы видите слова у других игроков, а сам(а) у себя нет, и должны отгадать его, и так кто быстрее.",
				user.Id))
			.WithColor(3093046)
			.AddPrizes(WhoIImage)
			.Build();
	}
	
	public static Embed GetCrocodileEmbed(this IUser user, long time)
	{
		return new EmbedBuilder()
			.WithTitle(GetTitle("Крокодил", time))
			.WithDescription(GetDescription("Крокодил",
				"Игра, где нужно отгадывать слова, которые изображают другие игроки с помощью кисти и набора красок.",
				user.Id))
			.WithColor(3093046)
			.AddPrizes(CrocodileImage)
			.Build();
	}
	
	public static Embed GetJackboxEmbed(this IUser user, long time)
	{
		return new EmbedBuilder()
			.WithTitle(GetTitle("Jackbox", time))
			.WithDescription(GetDescription("Джекбокс",
				"Серия групповых онлайн игр. Чтобы играть, требуется один человек для её запуска. " +
				"Остальным достаточно видеть экран и подключиться с других устройств через специальный [сайт](https://jackbox.fun/).",
				user.Id))
			.WithColor(3093046)
			.AddPrizes(JackboxImage)
			.Build();
	}
	
	public static Embed GetTabletopEmbed(this IUser user, long time)
	{
		return new EmbedBuilder()
			.WithTitle(GetTitle("Настолки", time))
			.WithDescription(GetDescription("Настольные игры",
				"Старые добрые настольные игры в новой обертке.",
				user.Id))
			.WithColor(3093046)
			.AddPrizes(TabletopImage)
			.Build();
	}
	
	public static Embed GetMomentsEmbed(this IUser user, long time)
	{
		return new EmbedBuilder()
			.WithTitle(GetTitle("Моменты", time))
			.WithDescription(GetDescription("Моменты",
				"Игра, построена на том, что нужно угадывать аниме по скриншоту. " +
				"Ведущий будет кидать, заранее заготовленные скрины в ветку, кто первый скажет название аниме, тот выиграет.",
				user.Id))
			.WithColor(3093046)
			.AddPrizes(MomentsImage)
			.Build();
	}

	private static EmbedBuilder AddPrizes(this EmbedBuilder embed, string image, Prize[] prizes)
	{
		embed.WithImageUrl(image);
		
		var first = prizes[0];
		var firstName = first.Name;
		var firstValue = first.Value.ToString();
		var second = prizes[1];
		var secondName = second.Name;
		var secondValue = second.Value.ToString();
		var third = prizes[2];
		var thirdName = third.Name;
		var thirdValue = third.Value.ToString();

		if (prizes.Length != 4)
			return embed.AddField(Empty, $"**{firstName} - {firstValue}{HeartVal}**", true)
				.AddField(Empty, $"**{secondName} - {secondValue}{HeartVal}**", true)
				.AddField(Empty, $"**{thirdName} - {thirdValue}{HeartVal}**", true);
		
		var fourth = prizes[3];
		var fourthName = fourth.Name;
		var fourthValue = fourth.Value.ToString();
		
		return embed.AddField(Empty,
				$"**{firstName} - {firstValue}{HeartVal}**\n**{secondName} - {secondValue}{HeartVal}**",
				true)
			.AddField(Empty,
				$"**{thirdName} - {thirdValue}{HeartVal}**\n**{fourthName} - {fourthValue}{HeartVal}**",
				true);
	}
	private static EmbedBuilder AddPrizes(this EmbedBuilder embed, string image, byte[] prizes)
	{
		return embed.AddPrizes(image, new []
		{
			new Prize("1 место", prizes[0]),
			new Prize("2 место", prizes[1]),
			new Prize("3 место", prizes[2]),
			new Prize("участие", prizes[3])
		});
	}
	
	private static EmbedBuilder AddPrizes(this EmbedBuilder embed, string image)
	{
		return embed.AddPrizes(image, new byte[]
		{
			150,
			100,
			60,
			40
		});
	}
	
	private class Prize
	{
		public string Name { get; set; }
		public byte Value { get; set; }
		
		public Prize(string name, byte value)
		{
			Name = name;
			Value = value;
		}
	}
}

