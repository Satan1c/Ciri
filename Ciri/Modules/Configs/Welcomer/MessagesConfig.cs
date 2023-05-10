using Discord;
using MessageProperties = Ciri.Models.MessageProperties;

namespace Ciri.Modules.Configs.Welcomer;

public static class MessagesConfig
{
    public static readonly ulong[] Roles = new ulong[]
    {
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9
    };
    
    public static readonly MessageProperties WelcomeProperties = new()
    {
        Embeds = new[]
        {
            new EmbedBuilder()
                .WithColor(3092790)
                .WithImageUrl(
                    "https://media.discordapp.net/attachments/696783977004138627/804014501146001418/dobro.png")
                .Build(),
            new EmbedBuilder()
                .WithDescription("**Приветствуем тебя на нашем сервере.**" +
                                 "\n\nСпасибо, что выбрал именно нас. Тут есть много интересного, ты это еще увидишь. " +
                                 "Но перед тем как ты начнешь общаться, предлагаю познакомиться с нашим сервером." +
                                 "\n\nЧтобы начать, **выберите раздел**.")
                .WithColor(3092790)
                .Build()
        },
        Components = new ComponentBuilder()
            .WithSelectMenu("welcome_category", new List<SelectMenuOptionBuilder>(new []
            {
                new SelectMenuOptionBuilder().WithLabel("🎭Роли").WithDescription("получить роли").WithValue("roles"),
                new SelectMenuOptionBuilder().WithLabel("🗺️Карта сервера").WithDescription("гайд по серверу").WithValue("map"),
                new SelectMenuOptionBuilder().WithLabel("🤖Команды").WithDescription("команды ботов").WithValue("bots"),
                new SelectMenuOptionBuilder().WithLabel("💰Донат").WithDescription("поддержка сервера").WithValue("donate")
            }))
            .Build()
    };

    public static readonly MessageProperties RolesProperties = new()
    {
        Embeds = new[]
        {
            new EmbedBuilder()
                .WithColor(3092790)
                .WithImageUrl("https://cdn.discordapp.com/attachments/689528587917066282/1089493517727977582/game-role.png")
                .Build(),
            new EmbedBuilder()
                .WithColor(3092790)
                .WithDescription("Вы можете получить игровые роли, которые помогут вам найти тиммейтов. Для этого выберите их из списка ниже.")
                .Build()
        },
        
        Components = new ComponentBuilder()
            .WithSelectMenu("roles_select", maxValues: 10, options: new List<SelectMenuOptionBuilder>(new []
            {
                new SelectMenuOptionBuilder()
                    .WithLabel("OSU!")
                    .WithValue(Roles[0].ToString())
                    .WithEmote(Emote.Parse("<:Osu:1089498581955772527>")),
                new SelectMenuOptionBuilder()
                    .WithLabel("Fortnite")
                    .WithValue(Roles[1].ToString())
                    .WithEmote(Emote.Parse("<:fortnite:1089498082519023657>")),
                new SelectMenuOptionBuilder()
                    .WithLabel("Dota 2")
                    .WithValue(Roles[2].ToString())
                    .WithEmote(Emote.Parse("<:dota:1089495569048805436>")),
                new SelectMenuOptionBuilder()
                    .WithLabel("CS")
                    .WithValue(Roles[3].ToString())
                    .WithEmote(Emote.Parse("<:cs:1089497250075516928>")),
                new SelectMenuOptionBuilder()
                    .WithLabel("Valorant")
                    .WithValue(Roles[4].ToString())
                    .WithEmote(Emote.Parse("<:valorant:1089496707529703465>")),
                new SelectMenuOptionBuilder()
                    .WithLabel("League of Legends")
                    .WithValue(Roles[5].ToString())
                    .WithEmote(Emote.Parse("<:lol:1089495735596224512>")),
                new SelectMenuOptionBuilder()
                    .WithLabel("Apex Legends")
                    .WithValue(Roles[6].ToString())
                    .WithEmote(Emote.Parse("<:apex:1089497506083254282>")),
                new SelectMenuOptionBuilder()
                    .WithLabel("Genshin Impact")
                    .WithValue(Roles[7].ToString())
                    .WithEmote(Emote.Parse("<:GenshinImpact:1089495684677382164>")),
                new SelectMenuOptionBuilder()
                    .WithLabel("Minecraft")
                    .WithValue(Roles[8].ToString())
                    .WithEmote(Emote.Parse("<:minecraft:1089497736832880730>")),
                new SelectMenuOptionBuilder()
                    .WithLabel("Other games")
                    .WithValue(Roles[9].ToString())
                    .WithEmote(Emote.Parse("<:othergames:1089495610526273566>")),
            }))
            .Build()
    };

    public static readonly MessageProperties MapProperties = new()
    {
        Embeds = new[]
        {
            new EmbedBuilder()
                .WithColor(3093046)
                .WithImageUrl("https://media.discordapp.net/attachments/687379079229341766/700817559909564565/roles.png?width=1368&height=683")
                .Build(),
            new EmbedBuilder()
                .WithColor(3093046)
                .WithTitle("<a:21414152:769546254522253355> Главные админские роли:")
                .WithDescription("⠀⠀<@&542017661341794304> — главные админы;" +
                                 "\n⠀⠀<@&698496217671401482> — разработчики ботов;" +
                                 "\n⠀⠀<@&542017417837281280> — старшие модераторы;" +
                                 "\n⠀⠀<@&803921489443029002> — младшие модераторы;" +
                                 "\n⠀⠀<@&686895612020654108> — организаторы ивентов;" +
                                 "\n⠀⠀<@&639707864591630337> — помощники сервера;")
                .Build(),
            new EmbedBuilder()
                .WithColor(3093046)
                .WithTitle("<a:21414152:769546254522253355> Активные:")
                .WithDescription("⠀⠀ <@&542016825219612672> — друзья и знакомые администрации или заслужившие доверие." +
                                 "\n⠀⠀ <@&654740376036311053> — люди, которые больше месяца общаются на сервере." +
                                 "\n⠀⠀ <@&542012055775870976> — новички нашего сервера.")
                .Build(),
            new EmbedBuilder()
                .WithColor(3093046)
                .WithTitle("<a:21414152:769546254522253355> Кастомные:")
                .WithDescription("⠀⠀ <@&689686456750571566> — киберкотлеты нашего сервера." +
                                 "\n⠀⠀ <@&634793820092891136> — люди, которые увлекаются аниме." +
                                 "\n⠀⠀ <@&691713934612496484> — люди, у которых более 100 подписчиков на youtube." +
                                 "\n⠀⠀ <@&691713849501417503> — люди, которые стримели более 5 часов." +
                                 " [Получить!](https://discord.gg/CTsmd26)" +
                                 "\n⠀⠀ <@&725689362624413797> — люди, которые постят мемы." +
                                 " [Получить!](https://discord.com/channels/542005378049638400/691573782153789540)" +
                                 "\n⠀⠀ <@&725689366051291166> — люди, которые имеют музыкальный талант." +
                                 "\n⠀⠀ <@&725689370446659666> — люди, которые постят арты/имеют творческий талант." +
                                 " [Получить!](https://discord.com/channels/542005378049638400/703569657390694410)" +
                                 "\n⠀⠀ <@&725689368735514768> — люди, которые постят эстетику." +
                                 " [Получить!](https://discord.com/channels/542005378049638400/688082279314096211)" +
                                 "\n⠀⠀ <@&725690488275075152> — за активность в войсах." +
                                 " [Получить!](https://discord.gg/67ywSMa)" +
                                 "\n⠀⠀ <@&725690491705884783> — за активность в такстовых чатах." +
                                 " [Получить!](https://discord.com/channels/542005378049638400/542005378049638403)" +
                                 "\n⠀⠀ <@&725691927953014874> — за 5 качественных предложений." +
                                 " [Получить!](https://discord.com/channels/542005378049638400/1080907815704608859)")
                .Build(),
            new EmbedBuilder()
                .WithColor(3093046)
                .WithTitle("<a:21414152:769546254522253355> Игровые:")
                .WithDescription("<@&700033105339351050>, <@&688440845048873011>, <@&686537665746567209>," +
                                 " <@&686537766296879115>, <@&700031124683882526>, <@&700030561766473808>," +
                                 " <@&686537928612118548>, <@&686537799419035668>, <@&700030557245014026>," +
                                 " <@&700030550475538502>, <@&700030554002817095>, <@&700030559744950312>." +
                                 "\n\nВсе эти роли можно взять в канале <#684010692571037706> в разделе \"Роли\"." +
                                 " Ещё с помощью этих ролей можно найти себе тимейта/тимейтов - <#1080781483674976276>" +
                                 "\n\nЕсли вы играете в какую-то из этих игр и она отображается в дискорде," +
                                 " то вам автоматически дается роль, на то время, пока вы не перестанете играть.")
                .Build(),
            new EmbedBuilder()
                .WithColor(3093046)
                .WithTitle("<a:21414152:769546254522253355> Гендерные:")
                .WithDescription("<@&691312169836347502> — парень;" +
                                 "\n<@&700015937528791111> — иной;" +
                                 "\n<@&691311950277115904> — девушка;" +
                                 "\n\nВсе эти роли можно взять в канале #Каналы и роли")
                .Build()
        }
    };

    public static readonly MessageProperties BotsProperties = new()
    {
        Embeds = new []
        {
            new EmbedBuilder()
                .WithColor(3093046)
                .WithDescription("Эта категория еще не готова")
                .Build()
        }
    };

    public static readonly MessageProperties DonateProperties = new()
    {
        Embeds = new []
        {
            new EmbedBuilder()
                .WithColor(3093046)
                .WithImageUrl("https://media.discordapp.net/attachments/696783977004138627/804014503809253416/podderzhka.png")
                .Build(),
            new EmbedBuilder()
                .WithColor(3093046)
                .WithTitle("<a:znak:781849766816120852> Поддержка Friendly team'a")
                .WithDescription("Мы обещаем, что ваши деньги пойдут на развитие сервера. Чтобы повышать актив и на рекламу." +
                                 "\nВсе плюшки будут даваться на месяц." +
                                 "\n\n**От 99 рублей**" +
                                 " \n <a:zvezdochka:785687625285238784>-  роль <@&803949488364191765>" +
                                 "\n <a:zvezdochka:785687625285238784>- доступ в чат ⸨💉︙админ-разговоры" +
                                 "\n <a:zvezdochka:785687625285238784>- 1000💜" +
                                 "\n <a:zvezdochka:785687625285238784>- множитель xp x2" +
                                 "\n\n**От 149 рублей**" +
                                 "\n <a:zvezdochka:785687625285238784>- роль <@&803949477370265610>" +
                                 "\n <a:zvezdochka:785687625285238784>-  доступ в чат ⸨💉︙админ-разговоры" +
                                 "\n <a:zvezdochka:785687625285238784>- 5000💜" +
                                 "\n <a:zvezdochka:785687625285238784>- Личная роль на месяц (обратится к администрации)." +
                                 "\n <a:zvezdochka:785687625285238784>- множитель xp x4" +
                                 "\n\n**За буст сервера.**" +
                                 "\n <a:zvezdochka:785687625285238784>- роль <@&709738102394191984>" +
                                 "\n <a:zvezdochka:785687625285238784>- 5000💜" +
                                 "\n <a:zvezdochka:785687625285238784>- Личная роль на месяц (обратится к администрации)" +
                                 "\n <a:zvezdochka:785687625285238784>- Личная войс-комната")
                .WithFooter("Все покупки можно осуществить обратившись к @neick")
                .Build()
        }
    };
}   