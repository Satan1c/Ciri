﻿using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.DataBinder;
using Ciri.Handlers;
using Ciri.Modules.Configs;
using DataBase.Models;
using Discord;
using Discord.WebSocket;

namespace Ciri.Modules.Utils;

public static class Extensions
{
	public const string Empty = "\u200b";

	private static readonly Regex s_formatRegex =
		new(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
			RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

	public static string GetDisplayAvatar(this IGuildUser member,
		ImageFormat format = ImageFormat.Auto,
		ushort size = 512)
	{
		return member.GetDisplayAvatarUrl(format, size) ?? member.GetDefaultAvatarUrl();
	}

	public static string GetDisplayAvatarUrl(this IUser user, ImageFormat format = ImageFormat.Auto, ushort size = 512)
	{
		return user.GetAvatarUrl(format, size) ?? user.GetDefaultAvatarUrl();
	}

	public static void GenerateEmbeds(this Shop shop, ref LinkedList<EmbedBuilder> embeds)
	{
		var items = CollectionsMarshal.AsSpan(shop.Items);
		for (var i = 0; i < items.Length; i += 5)
		{
			var description = new StringBuilder();
			var embed = new EmbedBuilder()
				.WithTitle("Магазин")
				.WithColor(3093046);

			var lim = items.Length - i >= 5 ? 5 : items.Length;
			for (var j = 0; j < lim; j++)
			{
				var shopItem = items[i + j];
				description.Add(
					$"[{(shopItem.Index + 1).ToString()}] {shopItem.Name}\n{shop.GetCost(shopItem).ToString()}{EmojiConfig.HeartVal}");
			}

			embed.WithDescription(description.ToString());

			embeds.AddLast(embed);
		}
	}

	public static StringBuilder Add(this StringBuilder builder, string value, char separator = '\n')
	{
		return builder.Length == 0
			? builder.Append(value)
			: builder.Append(separator).Append(value);
	}

	public static ActionRowBuilder AddPagination(this ActionRowBuilder builder,
		byte max,
		string[] ids,
		bool disabled = false)
	{
		var disable = disabled ? disabled : max == 1;
		var style = disable ? ButtonStyle.Secondary : ButtonStyle.Primary;
		return builder.WithButton("<=", ids[0], style, disabled: disable)
			.WithButton("[X]", ids[1], ButtonStyle.Danger, disabled: disabled)
			.WithButton("=>", ids[2], style, disabled: disable);
	}

	public static ActionRowBuilder AddItems(this ActionRowBuilder builder,
		int page,
		Shop shop,
		DataBase.Models.Profile profile,
		bool disabled = false)
	{
		page--;
		var items = shop.Items.Take((page * 5)..(page * 5 + 5)).ToArray();
		var count = items.Length;
		for (var i = 0; i < count; i++)
		{
			var index = i + page * 5;
			var item = items.First(x => x.Index == index);
			builder.WithButton(
				$"[{(index + 1).ToString()}]",
				$"buy_{index.ToString()}",
				disabled: disabled
					? disabled
					: profile.Hearts < shop.GetCost(item)
					  || (profile.Inventory.Count > 0
					      && profile.Inventory.Contains($"{shop.Name}_{item.Name}_{item.Index.ToString()}")));
		}

		if (count >= 5) return builder;

		var lim = (byte)(5 - count);
		for (var i = 0; i < lim; i++)
			builder.WithButton(
				$"[{(i + count + page * 5 + 1).ToString()}]",
				$"buy_empty_{i.ToString()}",
				ButtonStyle.Secondary,
				disabled: true);

		return builder;
	}

	public static EmbedBuilder SetPage(this EmbedBuilder builder, int current, int max, DateTimeOffset closeAt)
	{
		if (max > 1)
			builder.WithFooter($"Страница {current.ToString()} / {max.ToString()}");
		return builder.WithDescription(
			$"Авто-закрытие <t:{closeAt.ToUnixTimeSeconds().ToString()}:R>\n\n{builder.Description}");
	}

	public static ComponentBuilder SetShopControls(this ComponentBuilder builder,
		int current,
		Shop shop,
		DataBase.Models.Profile profile,
		string[] ids,
		bool disabled = false)
	{
		return builder
			.AddRow(new ActionRowBuilder().AddPagination(1, ids, disabled))
			.AddRow(new ActionRowBuilder().AddItems(current, shop, profile, disabled));
	}

	public static async Task UpdateShop(this SocketInteraction interaction,
		EmbedBuilder current,
		int currentPage,
		int max,
		DateTimeOffset closeAt,
		string[] ids,
		DataBase.Models.Profile profile,
		Shop shop)
	{
		current.SetPage(currentPage, max, closeAt);
		await interaction.ModifyOriginalResponseAsync(options =>
		{
			options.Embed = current.Build();
			options.Components = new ComponentBuilder()
				.SetShopControls(currentPage, shop, profile, ids).Build();
		});
	}

	public static async Task CloseShop(this SocketInteraction interaction,
		Shop shop,
		int current,
		int count,
		string[] ids)
	{
		await interaction.ModifyOriginalResponseAsync(options =>
		{
			options.Components = new ComponentBuilder()
				.SetShopControls(current, shop, new DataBase.Models.Profile { Hearts = 0 }, ids, true).Build();
		});
	}

	public static EmbedBuilder MoveLeft(this EmbedBuilder current,
		ref int currentPage,
		ref int max,
		ref LinkedList<EmbedBuilder> embeds)
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

	public static EmbedBuilder MoveRight(this EmbedBuilder current,
		ref int currentPage,
		ref int max,
		ref LinkedList<EmbedBuilder> embeds)
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
	{
		return format.FormatWith(source, null);
	}

	public static string FormatWith<T>(this string format, T source, IFormatProvider? provider)
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
			catch
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