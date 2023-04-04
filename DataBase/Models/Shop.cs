using MongoDB.Bson.Serialization.Attributes;

namespace DataBase.Models;

public class Shop
{
	[BsonElement("_id")] public string Name { get; set; } = string.Empty;

	[BsonElement("discount")] public sbyte Discount { get; set; }

	[BsonElement("items")] public List<ShopItem> Items { get; set; } = new();

	public long GetCost(ShopItem item)
	{
		var cost = item.GetCost();
		return (long)Math.Round(cost * (1 - (double)Discount / 100), 0);
	}
}