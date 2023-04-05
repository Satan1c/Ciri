using Ciri.Handlers;
using DataBase.Models;
using Discord;
using Discord.Interactions;

namespace Ciri.Models;

public class ItemArg
{
	[ComplexParameterCtor]
	public ItemArg(byte index, string name, long cost, IRole item, sbyte discount = 0)
	{
		Index = index;
		Name = name;
		Cost = cost;
		Item = item;
		Discount = discount;
	}

	public byte Index { get; set; }
	public string Name { get; set; }
	public long Cost { get; set; }
	public IRole Item { get; set; }
	public sbyte Discount { get; set; }

	public ShopItem CreateShopItem(ulong item)
	{
		return new ShopItem
		{
			Index = Index,
			Name = Name,
			Cost = Cost,
			Item = item,
			Discount = Discount
		};
	}
}

public class NullableItemArg
{
	[ComplexParameterCtor]
	public NullableItemArg([Autocomplete(typeof(ItemAutocomplete))] byte index,
		IRole? item = default,
		string? name = null,
		long? cost = null,
		sbyte? discount = null)
	{
		Index = index;
		Name = name;
		Cost = cost;
		Item = item;
		Discount = discount;
	}

	public byte Index { get; set; }
	public string? Name { get; set; }
	public long? Cost { get; set; }
	public IRole? Item { get; set; }
	public sbyte? Discount { get; set; }

	public ShopItem CreateShopItem(ulong item, ShopItem? old = null)
	{
		return new ShopItem
		{
			Index = Index,
			Name = Name ?? old?.Name ?? string.Empty,
			Cost = Cost ?? old?.Cost ?? 0,
			Item = item,
			Discount = Discount ?? 0
		};
	}
}