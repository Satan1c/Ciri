using Discord;
using static Ciri.Modules.Utils.Extensions;

namespace Ciri.Modules.Utils;

public static class EventsExtensions
{
	private const string c_eventChannel = "https://discord.com/channels/542005378049638400/700739189477474334";
	private const string c_heartVal = "<:heartftvalyta:779022997331378196>";
	private const string c_star = "<a:zvezdochka:785687625285238784>";
	
	private const string c_mafiaImage = "https://images-ext-2.discordapp.net/external/HHcqJf83OAtgxlAMK5DWnoJE0HyJcn_cBn3JgevCM8M/https/images-ext-1.discordapp.net/external/8CfeMCv8vAqqbbbQ1Eqaf0o7seLGp4Vxrqyj3ZYNURI/%253Fwidth%253D1216%2526height%253D684/https/media.discordapp.net/attachments/767725811905593346/779325292954124298/-1.png?width=1214&height=683";
	private const string c_aliasImage = "https://images-ext-2.discordapp.net/external/o2c-iffYr-FdMEt4rPs6dsEFG2DCCqzwPN-82FQqArk/%3Fwidth%3D1186%26height%3D683/https/images-ext-2.discordapp.net/external/0G0tUp_W8cbm0rGqEFiBjW4pSDqAD8B36Otqlr1TzbY/%253Fwidth%253D1188%2526height%253D684/https/media.discordapp.net/attachments/691511839942508554/710512695241211995/238_20200514192314.png";
	private const string c_whoIImage = "https://media.discordapp.net/attachments/691511839942508554/718420458923491388/308_20200605150614.png?width=1186&height=683";
	private const string c_crocodileImage = "https://images-ext-1.discordapp.net/external/VEc5oQLU_znJ2ygkpt5eUkt0FrKr_ZLGXREJGi3Iw94/https/i.ibb.co/4gGQNcr/crocodile.png?width=1368&height=683";
	private const string c_jackboxImage = "https://media.discordapp.net/attachments/767725811905593346/779741553634574366/JackBox4-revive.png?width=1094&height=683";
	private const string c_tabletopImage = "https://media.discordapp.net/attachments/699990941439754371/1082293035095834624/nastolkievent.png?width=1368&height=683";
	private const string c_momentsImage = "https://media.discordapp.net/attachments/669887782117703701/781572552580530216/5ee6b1c039ce0f6b.png?width=1214&height=683";
	//private const string whoIImage = "";
	
	public static async Task SendEventEmbedAsync(this IInteractionContext context, Embed embed)
	{
		var message = (await context.Channel.SendMessageAsync(
			"<@&700009031879622767>",
			embed: embed,
			components: new ComponentBuilder()
				.WithButton("Голосовой канал", style: ButtonStyle.Link, url: "https://discord.gg/tC6eFDr")
				.Build()))!;
		await context.Interaction.RespondAsync("Sent event", ephemeral: true);
	}
	
	private static string GetTitle(string name, long time)
	{
		return $"{name} | <t:{time}:t>";
	}
	private static string GetDescription(string name, string description, ulong eventer)
	{
		return $"{c_star}**- Ведущий:** <@{eventer}>\n\n*Приглашаем тебя сыграть в [{name}]({c_eventChannel})*.\n{description}";
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
			.WithImageUrl(c_mafiaImage)
			.AddField(Empty, $"**мафии - 100{c_heartVal}**", true)
			.AddField(Empty, $"**мирных - 50{c_heartVal}**", true)
			.AddField(Empty, $"**участие - 20{c_heartVal}**", true)
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
			.WithImageUrl(c_aliasImage)
			.AddField($"1 место - 150{c_heartVal}", $"**2 место - 100{c_heartVal}**", true)
			.AddField($"3 место -  50{c_heartVal}", $"**участие - 20{c_heartVal}**", true)
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
			.WithImageUrl(c_whoIImage)
			.AddField($"1 место - 150{c_heartVal}", $"**2 место - 100{c_heartVal}**", true)
			.AddField($"3 место -  60{c_heartVal}", $"**участие - 40{c_heartVal}**", true)
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
			.WithImageUrl(c_crocodileImage)
			.AddField($"1 место - 150{c_heartVal}", $"**2 место - 100{c_heartVal}**", true)
			.AddField($"3 место -  60{c_heartVal}", $"**участие - 40{c_heartVal}**", true)
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
			.WithImageUrl(c_jackboxImage)
			.AddField($"1 место - 150{c_heartVal}", $"**2 место - 100{c_heartVal}**", true)
			.AddField($"3 место -  60{c_heartVal}", $"**участие - 40{c_heartVal}**", true)
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
			.WithImageUrl(c_tabletopImage)
			.AddField($"1 место - 150{c_heartVal}", $"**2 место - 100{c_heartVal}**", true)
			.AddField($"3 место -  60{c_heartVal}", $"**участие - 40{c_heartVal}**", true)
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
			.WithImageUrl(c_momentsImage)
			.AddField($"1 место - 150{c_heartVal}", $"**2 место - 100{c_heartVal}**", true)
			.AddField($"3 место -  60{c_heartVal}", $"**участие - 40{c_heartVal}**", true)
			.Build();
	}
}