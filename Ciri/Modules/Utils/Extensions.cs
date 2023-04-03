using System.Text;
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
	
	public static StringBuilder Add(this StringBuilder builder, string value, char separator = '\n')
	{
		return builder.Length == 0
			? builder.Append(value)
			: builder.Append(separator).Append(value);
	}

	public static ActionRowBuilder AddPagination(this ActionRowBuilder builder, byte max, string[] ids, bool disabled = false)
	{
		var disable = disabled ? disabled : max == 1;
		var style = disable ? ButtonStyle.Secondary : ButtonStyle.Primary;
		return builder.WithButton("<=", ids[0], style, disabled: disable)
			.WithButton("[X]", ids[1], ButtonStyle.Danger, disabled: disabled)
			.WithButton("=>", ids[2], style, disabled: disable);
	}

	public static ActionRowBuilder AddItems(this ActionRowBuilder builder, int page, long[] itemList, long wallet, bool disabled = false)
	{
		page--;
		var count = itemList.Length;
		for (var i = 0; i < count; i++)
			builder.WithButton(
				$"[{i + page * 5 + 1}]",
				$"buy_{i + page * 5}",
				disabled: disabled ? disabled : wallet < itemList[i]);

		if (count >= 5) return builder;

		var lim = (byte)(5 - count);
		for (var i = 0; i < lim; i++)
			builder.WithButton($"[{i + count + page * 5 + 1}]", $"buy_empty_{i}", ButtonStyle.Secondary, disabled: true);


		return builder;
	}

	public static EmbedBuilder SetPage(this EmbedBuilder builder, int current, int max, DateTimeOffset closeAt)
	{
		if (max > 1)
			builder.WithFooter($"Страница {current} / {max}");
		return builder.WithDescription($"Авто-закрытие <t:{closeAt.ToUnixTimeSeconds()}:R>");
	}

	public static ComponentBuilder SetShopControls(this ComponentBuilder builder, int current, long[] itemList, long wallet, string[] ids, bool disabled = false)
	{
		return builder
			.AddRow(new ActionRowBuilder().AddPagination(1, ids, disabled))
			.AddRow(new ActionRowBuilder().AddItems(current, itemList, wallet, disabled));
	}
	
	public static async Task<long[]> UpdateShop(this SocketInteraction interaction,
		EmbedBuilder current, int currentPage, int max,
		DateTimeOffset closeAt, long[] items, string[] ids,
		DataBase.Models.Profile profile, long[] currentItems)
	{
		current.SetPage(currentPage, max, closeAt);
		currentItems = items.Take((currentPage * 5)..(currentPage * 5 + 5)).ToArray();
		await interaction.ModifyOriginalResponseAsync(options =>
		{
			options.Embed = current.Build();
			options.Components = new ComponentBuilder()
				.SetShopControls(currentPage, currentItems, profile.Hearts, ids).Build();
		});

		return currentItems;
	}

	public static async Task CloseShop(this SocketInteraction interaction, int current, int count, string[] ids)
	{
		await interaction.ModifyOriginalResponseAsync(options =>
		{
			options.Components = new ComponentBuilder().SetShopControls(current, new long[count], -1, ids, true).Build();
		});
	}

	public static EmbedBuilder MoveLeft(this EmbedBuilder current, ref int currentPage, ref int max, ref LinkedList<EmbedBuilder> embeds)
	{
		if (currentPage == 1)
		{
			current = embeds.Last!.Value;
			currentPage = max;
		}
		else
		{
			current = embeds.Find(current)!.Previous!.Value;
			currentPage--;
		}

		return current;
	}
	public static EmbedBuilder MoveRight(this EmbedBuilder current, ref int currentPage, ref int max, ref LinkedList<EmbedBuilder> embeds)
	{
		if (currentPage == max)
		{
			current = embeds.First!.Value;
			currentPage = 1;
		}
		else
		{
			current = embeds.Find(current)!.Next!.Value;
			currentPage++;
		}
		
		return current;
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
				values.Add(propertyName.Value == "0"
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