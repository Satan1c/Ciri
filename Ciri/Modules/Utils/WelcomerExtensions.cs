using Discord;
using Discord.WebSocket;

namespace Ciri.Modules.Utils;

public static class WelcomerExtensions
{
	public static Embed GetWelcomeEmbed(this SocketGuildUser member)
	{
		return new EmbedBuilder()
			.WithColor(3093046)
			.WithTitle("🌞  🇯🇵  ヤーホー !  ( Yaho ! )  🇯🇵  🌞")
			.WithDescription($"Приветствуем тебя, <@{member.Id}>!" +
			                 "\n\n" +
			                 "Только что **Ты** стал частью уютного сервера" +
			                 "\n" +
			                 "FRIENDLY TEAM💜!" +
			                 "\n" +
			                 "👉 Обязательно загляни в канал <#684010692571037706>" +
			                 "\n\n" +
			                 "Там ты найдешь всю важную инфу для комфортного времяпровождения на сервере (⌒▽⌒)❤️" +
			                 "\n\n" +
			                 "💬 Если хочешь начать общение," +
			                 "\n" +
			                 "просто напиши \"Привет всем\" в канал <#542005378049638403>" +
			                 "\n\n" +
			                 $"Ты у нас уже - {member.Guild.MemberCount}-й гость.")
			.WithThumbnailUrl(member.GetDisplayAvatarUrl() ?? member.GetDefaultAvatarUrl())
			.Build();
	}
	
	public static Embed GetGoodbyeEmbed(this SocketUser user)
	{
		return new EmbedBuilder()
			.WithColor(3093046)
			.WithTitle("🌛  🇯🇵  さようなら!  ( Sayounara ! )  🇯🇵  🌛!")
			.WithDescription($"Береги себя, <@{user.Id}>!" +
			                 "\n\n" +
			                 "Бyдем надеяться, что наш дрyг снова зяглянет к нам! ✨.")
			.WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
			.Build();
	}
}