﻿using DataBase.Models;
using Discord;
using Discord.Interactions;

namespace Ciri.Models;

public class ItemArg<TItem>
{
	public byte Index { get; set; }
	public string Name { get; set; }
	public long Cost { get; set; }
	public TItem Item { get; set; }
	public sbyte Discount { get; set; }
			
	[ComplexParameterCtor]
	public ItemArg(byte index, string name, long cost, TItem item, sbyte discount = 0)
	{
		Index = index;
		Name = name;
		Cost = cost;
		Item = item;
		Discount = discount;
	}
	
	public ShopItem<T> CreateShopItem<T>(T item)
	{
		return new ShopItem<T>
		{
			Index = Index,
			Name = Name,
			Cost = Cost,
			Item = item,
			Discount = Discount
		};
	}
}
public class NullableItemArg<TItem>
{
	public byte Index { get; set; }
	public string? Name { get; set; }
	public long? Cost { get; set; }
	public TItem? Item { get; set; }
	public sbyte? Discount { get; set; }
			
	[ComplexParameterCtor]
	public NullableItemArg(byte index, TItem? item = default, string? name = null, long? cost = null, sbyte? discount = null)
	{
		Index = index;
		Name = name;
		Cost = cost;
		Item = item;
		Discount = discount;
	}
	
	public ShopItem<T> CreateShopItem<T>(T item, ShopItem<T>? old = null)
	{
		return new ShopItem<T>
		{
			Index = Index,
			Name = Name ?? old?.Name ?? string.Empty,
			Cost = Cost ?? old?.Cost ?? 0,
			Item = item,
			Discount = Discount ?? 0
		};
	}
}