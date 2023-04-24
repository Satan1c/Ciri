using MongoDB.Bson.Serialization.Attributes;

namespace DataBase.Models;

[BsonIgnoreExtraElements]
public struct ShopItem
{
	[BsonConstructor]
	public ShopItem(byte index = 0, string name = null!, long cost = 0, ulong item = 0, sbyte discount = 0)
	{
		Index = index;
		Name = name ?? string.Empty;
		Cost = cost;
		Item = item;
		Discount = discount;
	}

	[BsonElement("index")] public byte Index { get; set; }

	[BsonElement("name")] public string Name { get; set; } = string.Empty;

	[BsonElement("cost")] public long Cost { get; set; }

	[BsonElement("item")] public ulong Item { get; set; }

	[BsonElement("discount")] public sbyte Discount { get; set; }

	public long GetCost()
	{
		return (long)Math.Round(Cost * (1 - (double)Discount / 100), 0);
	}
	
	public static byte NormalizeIndex(byte index)
	{
		return (byte)(index == 0 ? 0 : index - 1);
	}
}