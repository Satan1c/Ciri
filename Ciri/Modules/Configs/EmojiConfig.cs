namespace Ciri.Modules.Configs;

public static class EmojiConfig
{
	public static EmojiConfigSingleton Instance { get; } = new ();
	public const string HeartVal = "<:heartftvalyta:779022997331378196>";
	public const string Star = "<a:zvezdochka:785687625285238784>";
}

public class EmojiConfigSingleton
{
	public string HeartVal => EmojiConfig.HeartVal;
	public string Star => EmojiConfig.Star;
}

