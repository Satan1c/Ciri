using MongoDB.Bson.Serialization.Attributes;

namespace DataBase.Models;

public struct Profile
{
	public static readonly Profile Default = GetDefault(0);

	public Profile()
	{
		Id = 0;
	}

	[BsonElement("_id")] public ulong Id { get; set; } = 0;
	[BsonElement("bio")] public string Bio { get; set; } = "";
	[BsonElement("hearts")] public long Hearts { get; set; } = 0;
	[BsonElement("reputation")] public long Reputation { get; set; } = 0;
	[BsonElement("messages")] public ulong Messages { get; set; } = 0;
	[BsonElement("lover")] public ulong? Lover { get; set; } = null;
	[BsonElement("inventory")] public List<string> Inventory { get; set; } = new();
	[BsonElement("voice")] public DateTime Voice { get; set; } = new();

	public static Profile GetDefault(ulong id)
	{
		var profile = new Profile();
		profile.Id = id;
		return profile;
	}
}