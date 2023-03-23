using Discord;
using MessageProperties = Ciri.Models.MessageProperties;

namespace Ciri.Modules.Configs.Requests;

public static class RequestsConfig
{
	public const ulong ModersThread = 1081311778539049061;
	public const ulong EventersThread = 1081311794603241515;
	public const string Tabs = "\t\t";

	public static readonly string[] Dates = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "\u200b", "Суббота", "Воскресенье", "\u200b" };

	public static readonly ButtonBuilder RequestButton = new ButtonBuilder()
		.WithLabel("Подать заявку")
		.WithStyle(ButtonStyle.Primary);
	public static readonly MessageProperties RequestsMessageProperties = new()
	{
		Embeds = new[]
		{
			new EmbedBuilder()
				.WithColor(3093046)
				.WithImageUrl("https://cdn.discordapp.com/attachments/689528587917066282/1080551361013354607/688e69382967bbf5.png")
				.Build(),
			new EmbedBuilder()
				.WithColor(3093046)
				.WithTitle("Friendly Team | Персонал")
				.WithImageUrl("https://cdn.discordapp.com/attachments/888386631403446282/1061378507445391400/3333.png")
				.WithDescription(@"""
<a:21414152:769546254522253355> **Хочется** попасть к нам в **команду** ?
<a:21414152:769546254522253355> **Выбери должность, которая тебе по душе**
"""[1..^2])
				.WithFooter("Если ваша заявка нас заинтересует, мы с вами свяжемся.")
				.Build()
		},
		Components = new ComponentBuilder()
			.WithSelectMenu(
				new SelectMenuBuilder(
					"staff_request_select",
					new List<SelectMenuOptionBuilder>(new[]
					{
						new SelectMenuOptionBuilder()
							.WithLabel("Модератор")
							.WithEmote(new Emoji("🛡️"))
							.WithValue("moder"),
						new SelectMenuOptionBuilder()
							.WithLabel("Ивентер")
							.WithEmote(new Emoji("🎈"))
							.WithValue("eventer"),
					})
				))
			.Build()
	};
	
	public static readonly MessageProperties EventersRequestMessageProperties = new()
	{
		Embed = new EmbedBuilder()
			.WithColor(3093046)
			.WithTitle("Набор в  HIROKI ߷ — вступаем в ряды ивентеров !")
			.WithDescription(@$"""
<a:heart2:783721812492025878> **Чем занимается ивентер ?**
{Tabs}От ивентера требуется проводить мероприятия, как минимум - **раз в неделю**.
{Tabs}Текста на ивенты он может отправлять сам (чему админы его научат), или же просить администрацию.
{Tabs}**Знать многие правила игр** (его этому тоже могут обучить) или же почитать их тут - <#700739189477474334>

<a:heart2:783721812492025878> **Какие плюсы y модераторов ?**
{Tabs}<a:zvezdochka:785687625285238784> Зарплата в виде <:heartftvalyta:779022997331378196> каждую неделю;
{Tabs}<a:zvezdochka:785687625285238784> Доступ в секретные админские чатики;
{Tabs}<a:zvezdochka:785687625285238784> Отличительная роль HIROKI ߷;
{Tabs}<a:zvezdochka:785687625285238784> Прокачка собственных soft skill'ов.

<a:heart2:783721812492025878> **Что требуется от тебя ?**
{Tabs}<a:zvezdochka:785687625285238784> Уделять серверу 2-4 часа в день;
{Tabs}<a:zvezdochka:785687625285238784> Адекватность, ответственность, стрессоустойчивость;
{Tabs}<a:zvezdochka:785687625285238784> 15 полных лет;
{Tabs}<a:zvezdochka:785687625285238784> Умение ладить с другими участниками сервера;

<a:heart2:783721812492025878> Хочy стать ивентером ! Что делать?
{Tabs}{Tabs}**Нажимай на кнопку ниже!**
"""[1..^2])
			.WithFooter("Будем ждать тебя в нашем дружном коллективе !")
			.WithImageUrl("https://cdn.discordapp.com/attachments/689528587917066282/1081260389079207966/iventerr-recruitment.png")
			.Build(),
		
		Components = new ComponentBuilder()
			.WithButton(RequestButton.WithCustomId("eventer_request"))
			.Build()
	};
	
	public static readonly MessageProperties ModersRequestMessageProperties = new()
	{
		Embed = new EmbedBuilder()
			.WithColor(3093046)
			.WithTitle("Набор в  TEMOTSU ߷  — вступаем в модераторы !")
			.WithDescription(@$"""
<a:heart2:783721812492025878> **Чем занимается модератор ?**
{Tabs}Модератор следит за общим порядком в текстовых/голосовых каналах, опираясь на правила сервера, и выдаёт предyпреждения/мyт нарyшителям.
{Tabs}Если __ты часто зависаешь на сервере__, эта роль точно для тебя!

<a:heart2:783721812492025878> **Какие плюсы y модераторов ?**
{Tabs}<a:zvezdochka:785687625285238784> Зарплата в виде <:heartftvalyta:779022997331378196> каждую неделю;
{Tabs}<a:zvezdochka:785687625285238784> Доступ в секретные админские чатики;
{Tabs}<a:zvezdochka:785687625285238784> Отличительная роль FUTERRY ߷ / TEMOTSU ߷;
{Tabs}<a:zvezdochka:785687625285238784> Прокачка собственных soft skill'ов

<a:heart2:783721812492025878> **Что требуется от тебя ?**
{Tabs}<a:zvezdochka:785687625285238784> Уделять серверу 1-2 часа в день;
{Tabs}<a:zvezdochka:785687625285238784> Адекватность и ответственность;
{Tabs}<a:zvezdochka:785687625285238784> 14 полных лет;
{Tabs}<a:zvezdochka:785687625285238784> Роль <@&654740376036311053> (или выше).

В самом начале вашего пути, на **испытательный срок**  вам выдадут роль *FUTERRY ߷* , то есть младшего модератора, если вы хорошо будете справляться с __предоставленной работой__, будут повышения.

<a:heart2:783721812492025878> **Хочy стать модератором ! Что делать ?**
{Tabs}{Tabs}**Нажимай на кнопку ниже!**
"""[1..^2])
			.WithFooter("Будем ждать тебя в нашем дружном коллективе !")
			.WithImageUrl("https://cdn.discordapp.com/attachments/689528587917066282/1081260690792272012/moderator-recruitment.png")
			.Build(),
		
		Components = new ComponentBuilder()
			.WithButton(RequestButton.WithCustomId("moder_request"))
			.Build()
	};
}