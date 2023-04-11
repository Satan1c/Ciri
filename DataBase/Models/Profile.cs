using MongoDB.Bson.Serialization.Attributes;

namespace DataBase.Models;

public struct Profile
{
	public Profile()
	{
	}

	public Profile(ulong id = 0,
		string bio = "",
		long hearts = 0,
		long reputation = 0,
		ulong messages = 0,
		ulong? lover = null,
		List<string>? inventory = null,
		DateTime? voice = null)
	{
		Id = id;
		Bio = bio;
		Hearts = hearts;
		Reputation = reputation;
		Messages = messages;
		Lover = lover;
		Inventory = inventory ?? new List<string>();
		Voice = voice ?? new DateTime();
	}

	[BsonElement("_id")] public ulong Id { get; set; } = 0;
	[BsonElement("bio")] public string Bio { get; set; } = "";
	[BsonElement("hearts")] public long Hearts { get; set; } = 0;
	[BsonElement("reputation")] public long Reputation { get; set; } = 0;
	[BsonElement("messages")] public ulong Messages { get; set; } = 0;
	[BsonElement("lover")] public ulong? Lover { get; set; } = null;
	[BsonElement("inventory")] public List<string> Inventory { get; set; } = new();
	[BsonElement("voice")] public DateTime Voice { get; set; } = new();
}