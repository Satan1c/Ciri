using MongoDB.Bson.Serialization.Attributes;

namespace DataBase.Models;

[BsonIgnoreExtraElements]
public struct Shop
{
	[BsonConstructor]
	public Shop(string name = "roles",
		sbyte discount = 0,
		List<ShopItem> items = null!)
	{
		Name = name;
		Discount = discount;
		Items = items ?? new List<ShopItem>();
	}

	[BsonElement("_id")] public string Name { get; set; } = "roles";

	[BsonElement("discount")] public sbyte Discount { get; set; }

	[BsonElement("items")] public List<ShopItem> Items { get; set; } = new();

	public long GetCost(ShopItem item)
	{
		var cost = item.GetCost();
		return (long)Math.Round(cost * (1 - (double)Discount / 100), 0);
	}

	public Shop GetCopy()
	{
		var items = new ShopItem[Items.Count];
		Items.CopyTo(items);
		return new Shop(Name, Discount, new List<ShopItem>(items));
	}
}