using MongoDB.Bson.Serialization.Attributes;

namespace DataBase.Models;

public class Profile
{
	[BsonElement("_id")] public ulong Id { get; set; } = 0;
	[BsonElement("bio")] public string Bio { get; set; } = "";
	[BsonElement("hearts")] public long Hearts { get; set; } = 0;
	[BsonElement("reputation")] public long Reputation { get; set; } = 0;
	[BsonElement("messages")] public ulong Messages { get; set; } = 0;
	[BsonElement("lover")] public ulong Lover { get; set; } = 0;
	[BsonElement("voice")] public DateTime Voice { get; set; } = new();
	
	public static readonly Profile Default = GetDefault(0);

	public static Profile GetDefault(ulong id)
	{
		var profile = new Profile();
		profile.Id = id;
		return profile;
	}
}