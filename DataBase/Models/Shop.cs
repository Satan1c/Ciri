using MongoDB.Bson.Serialization.Attributes;

namespace DataBase.Models;

public class Shop<TItem>
{
	[BsonElement("_id")] public string Name { get; set; } = string.Empty;

	[BsonElement("discount")] public sbyte Discount { get; set; }

	[BsonElement("items")] public List<ShopItem<TItem>> Items { get; set; } = new();

	public long GetCost(ShopItem<TItem> item)
	{
		var cost = item.GetCost();
		return (long)Math.Round(cost * (1 - (double)Discount / 100), 0);
	}
}