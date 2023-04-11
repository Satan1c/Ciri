using MongoDB.Bson.Serialization.Attributes;

namespace DataBase.Models;

public class Shop
{
	public Shop()
	{
	}

	public Shop(string name = "shop",
		sbyte discount = 0,
		List<ShopItem>? items = null)
	{
		Name = name;
		Discount = discount;
		Items = items ?? new List<ShopItem>();
	}

	[BsonElement("_id")] public string Name { get; set; }

	[BsonElement("discount")] public sbyte Discount { get; set; }

	[BsonElement("items")] public List<ShopItem> Items { get; set; }

	public long GetCost(ShopItem item)
	{
		var cost = item.GetCost();
		return (long)Math.Round(cost * (1 - (double)Discount / 100), 0);
	}
}