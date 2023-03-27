using CacheManager.Core;
using DataBase.Models;
using MongoDB.Driver;

namespace DataBase;

public class DataBaseProvider : IDataBaseProvider
{
	private readonly ICacheManager<Profile> m_cache;
	private readonly IMongoCollection<Profile> m_profiles;

	public DataBaseProvider(MongoClient client) : this(client as IMongoClient) { }
	public DataBaseProvider(IMongoClient client)
	{
		m_cache = CacheFactory.Build<Profile>(part =>
			part.WithMicrosoftMemoryCacheHandle().WithExpiration(ExpirationMode.Sliding, TimeSpan.FromMinutes(60)));
		
		var database = client.GetDatabase("main");
		m_profiles = database.GetCollection<Profile>("profiles");
	}

	public async Task SyncCache()
	{
		var filter = Builders<Profile>.Filter.Empty;
		var options = new FindOptions<Profile> { BatchSize = 10 };
		var cursor = await m_profiles.FindAsync(filter, options);
		while (await cursor.MoveNextAsync())
		{
			foreach (var profile in cursor.Current)
			{
				m_cache.Put(profile.Id.ToString(), profile);
			}
		}
	}
	
	public async Task<bool> HasProfile(ulong id)
	{
		var exists = m_cache.Exists(id.ToString());
		if (exists) return exists;
		
		var finded = await (await m_profiles.FindAsync(profile => profile.Id == id)).FirstOrDefaultAsync();
		
		if (finded == null) return exists;
		exists = true;
		
		m_cache.Put(finded.Id.ToString(), finded);

		return exists;
	}
	
	public async Task<Profile> GetProfile(ulong id, bool fetch = true)
	{
		var profile = Profile.GetDefault(id);
		
		if (m_cache.TryGetOrAdd(id.ToString(), s => profile, out profile))
		{
			return profile;
		}
		
		if (!fetch) return profile;
		
		var filter = await (await m_profiles.FindAsync(profile1 => profile1.Id == id)).FirstOrDefaultAsync();
		if (filter != null)
			profile = filter;
		
		return profile;
	}
	
	public async Task AddOrUpdateProfile(Profile profile)
	{
		m_cache.Put(profile.Id.ToString(), profile);
		
		if (await m_profiles.FindOneAndReplaceAsync(profile1 => profile1.Id == profile.Id, profile) != null)
		{
			return;
		}
		
		await m_profiles.InsertOneAsync(profile);
	}
}