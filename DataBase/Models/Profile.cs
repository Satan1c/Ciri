using MongoDB.Bson.Serialization.Attributes;

namespace DataBase.Models;

public class Profile
{
	public Profile()
	{
	}
  
	public Profile(ulong id = 0,
		string? bio = null,
		long hearts = 0,
		long reputation = 0,
		ulong messages = 0,
		ulong? lover = null,
		List<string>? inventory = null,
		DateTime? voice = null)
	{
		Id = id;
		Bio = bio ?? string.Empty;
		Hearts = hearts;
		Reputation = reputation;
		Messages = messages;
		Lover = lover;
		Inventory = inventory ?? new List<string>();
		Voice = voice ?? new DateTime();
	}

	[BsonElement("_id")] public ulong Id { get; set; }
	[BsonElement("bio")] public string Bio { get; set; }
	[BsonElement("hearts")] public long Hearts { get; set; }
	[BsonElement("reputation")] public long Reputation { get; set; }
	[BsonElement("messages")] public ulong Messages { get; set; }
	[BsonElement("lover")] public ulong? Lover { get; set; }
	[BsonElement("inventory")] public List<string> Inventory { get; set; }
	[BsonElement("voice")] public DateTime Voice { get; set; }
}