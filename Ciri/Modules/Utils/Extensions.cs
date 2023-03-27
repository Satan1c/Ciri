using System.Text.RegularExpressions;
using System.Web.UI.DataBinder;
using Ciri.Handlers;
using Ciri.Modules.Configs;
using Discord;
using Discord.WebSocket;

namespace Ciri.Modules.Utils;

public static class Extensions
{
	private static readonly Regex s_formatRegex =
		new(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
			RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
	public const string Empty = "\u200b";

	public static string GetDisplayAvatar(this IGuildUser member, ImageFormat format = ImageFormat.Auto, ushort size = 512)
	{
		return member.GetDisplayAvatarUrl(format, size) ?? member.GetDefaultAvatarUrl();
	}
	public static string GetDisplayAvatarUrl(this IUser user, ImageFormat format = ImageFormat.Auto, ushort size = 512)
	{
		return user.GetAvatarUrl(format, size) ?? user.GetDefaultAvatarUrl();
	}
	
	public static string FormatWith<T>(this string format, T source)
		where T : class
	{
		return format.FormatWith(source, null);
	}
	
	public static string FormatWith<T>(this string format, T source, IFormatProvider? provider)
	where T: class
	{
		if (format == null)
			throw new ArgumentNullException(nameof(format));
		
		var values = new List<object>();
		var rewrittenFormat = s_formatRegex.Replace(format, m =>
		{
			var leftBracket = m.Groups["start"];
			var propertyName = m.Groups["property"];
			var formatGroup = m.Groups["format"];
			var rightBracket = m.Groups["end"];

			try
			{
				values.Add((propertyName.Value == "0")
					? source
					: DataBinder.Eval(source, propertyName.Value));
			}
			catch (Exception _)
			{
				values.Add(DataBinder.Eval(EmojiConfig.Instance, propertyName.Value));
			}
			
			return new string('{', leftBracket.Captures.Count) +
			       (values.Count - 1) +
			       formatGroup.Value +
			       new string('}', rightBracket.Captures.Count);
		});

		try
		{
			var res = values.Count == 0
				? format
				: string.Format(provider, rewrittenFormat, values.ToArray());

			return res;
		}
		catch (Exception e)
		{
			ClientEvents.OnLog(
				new LogMessage(
					LogSeverity.Error,
					nameof(FormatWith),
					$"rewrittenFormat: {rewrittenFormat} values: {string.Join(',', values)}",
					e));
		}

		return rewrittenFormat;
	}
}