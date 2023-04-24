using MongoDB.Bson.Serialization.Attributes;

namespace DataBase.Models;

[BsonIgnoreExtraElements]
public struct Profile
{
	[BsonConstructor]
	public Profile(ulong id = 0,
		string bio = null!,
		long hearts = 0,
		long reputation = 0,
		ulong messages = 0,
		ulong? lover = null,
		LinkedList<string> inventory = null!,
		LinkedList<ulong> repGiven = null!,
		DateTime voice = default)
	{
		Id = id;
		Bio = bio ?? string.Empty;
		Hearts = hearts;
		Reputation = reputation;
		Messages = messages;
		Lover = lover;
		Inventory = inventory ?? new LinkedList<string>();
		RepGiven = repGiven ?? new LinkedList<ulong>();
		Voice = voice;
	}
	
	[BsonElement("_id")] public ulong Id { get; set; }
	[BsonElement("bio")] public string Bio { get; set; } = string.Empty;
	[BsonElement("hearts")] public long Hearts { get; set; }
	[BsonElement("reputation")] public long Reputation { get; set; }
	[BsonElement("messages")] public ulong Messages { get; set; }
	[BsonElement("lover")] public ulong? Lover { get; set; }
	[BsonElement("inventory")] public LinkedList<string> Inventory { get; set; } = new();
	[BsonElement("rep_given")] public LinkedList<ulong> RepGiven { get; set; } = new();
	[BsonElement("voice")] public DateTime Voice { get; set; }
}