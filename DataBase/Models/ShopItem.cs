using MongoDB.Bson.Serialization.Attributes;

namespace DataBase.Models;

public class ShopItem<TItem>
{
	[BsonElement("index")] public byte Index { get; set; }

	[BsonElement("name")] public string Name { get; set; } = string.Empty;

	[BsonElement("cost")] public long Cost { get; set; }

	[BsonElement("item")] public TItem Item { get; set; } = default!;

	[BsonElement("discount")] public sbyte Discount { get; set; }

	public long GetCost()
	{
		return (long)Math.Round(Cost * (1 - (double)Discount / 100), 0);
	}
}