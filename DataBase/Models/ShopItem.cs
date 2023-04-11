using MongoDB.Bson.Serialization.Attributes;

namespace DataBase.Models;

public struct ShopItem
{
	public ShopItem()
	{
	}

	[BsonConstructor]
	public ShopItem(byte index = 0, string name = "", long cost = 0, ulong item = 0, sbyte discount = 0)
	{
		Index = index;
		Name = name;
		Cost = cost;
		Item = item;
		Discount = discount;
	}

	[BsonElement("index")] public byte Index { get; set; } = 0;

	[BsonElement("name")] public string Name { get; set; } = string.Empty;

	[BsonElement("cost")] public long Cost { get; set; } = 0;

	[BsonElement("item")] public ulong Item { get; set; } = default!;

	[BsonElement("discount")] public sbyte Discount { get; set; } = 0;

	public long GetCost()
	{
		return (long)Math.Round(Cost * (1 - (double)Discount / 100), 0);
	}
}