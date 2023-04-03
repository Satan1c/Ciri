using CacheManager.Core;
using DataBase.Models;
using MongoDB.Driver;

namespace DataBase;

public class DataBaseProvider
{
	private readonly ICacheManager<Profile> m_profileCache;
	private readonly ICacheManager<object> m_shopCache;
	private readonly ICacheManager<object> m_shopItemCache;
	
	private readonly IMongoDatabase m_database;
	private readonly IMongoCollection<Profile> m_profiles;

	public DataBaseProvider(MongoClient client) : this(client as IMongoClient) { }
	public DataBaseProvider(IMongoClient client)
	{
		m_profileCache = CacheFactory.Build<Profile>(part =>
			part.WithMicrosoftMemoryCacheHandle()
				.WithExpiration(ExpirationMode.Sliding, TimeSpan.FromMinutes(60)));
		
		m_shopCache = CacheFactory.Build(settings =>
			settings.WithMicrosoftMemoryCacheHandle()
				.WithExpiration(ExpirationMode.Sliding, TimeSpan.FromDays(7)));
		
		m_shopItemCache = CacheFactory.Build(part =>
			part.WithMicrosoftMemoryCacheHandle()
				.WithExpiration(ExpirationMode.Sliding, TimeSpan.FromDays(3)));
		
		m_database = client.GetDatabase("main");
		m_profiles = m_database.GetCollection<Profile>("profiles");
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
				m_profileCache.Put(profile.Id.ToString(), profile);
			}
		}
	}
	
	public async Task<bool> HasProfile(ulong id)
	{
		var exists = m_profileCache.Exists(id.ToString());
		if (exists) return true;
		
		var profile = await GetProfiles(id);
		
		if (profile == Profile.Default) return false;
		exists = true;

		return exists;
	}
	
	public async Task<Profile> GetProfiles(ulong id)
	{
		var profileId = id.ToString();
		if (m_profileCache.Exists(profileId))
		{
			return m_profileCache.Get(profileId);
		}

		var filter = await (await m_profiles.FindAsync(profile1 => profile1.Id == id)).FirstOrDefaultAsync();
		if (filter == null) return Profile.GetDefault(id);
		
		m_profileCache.Put(profileId, filter);

		return filter;
	}
	public async Task<Profile[]> GetProfiles(ulong[] ids)
	{
		var profiles = new LinkedList<Profile>();
		foreach (var id in ids)
		{
			var profileId = id.ToString();
			if (m_profileCache.Exists(profileId))
			{
				profiles.AddLast(m_profileCache.Get(profileId));
			}
		}
		
		if (profiles.Count == ids.Length) return profiles.ToArray();

		var filter = await m_profiles.FindAsync(profile1 => ids.Contains(profile1.Id) && !profiles.Contains(profile1));
		foreach (var profile in await filter.ToListAsync())
		{
			profiles.AddLast(profile);
			m_profileCache.Put(profile.Id.ToString(), profile);
		}

		return profiles.ToArray();
	}

	public async Task<Shop<TItem>?> GetShop<TItem>(string name = "roles")
	{
		if (m_shopCache.Exists(name, "shop"))
		{
			return (Shop<TItem>?) m_shopCache.Get(name, "shop");
		}

		var filter = await (await m_database.GetCollection<Shop<TItem>>("shop")
				.FindAsync(shop => shop.Name == name)
			).FirstOrDefaultAsync();
		
		if (filter == null) return default;
		
		m_shopCache.Put(name, filter, "shop");

		return filter;
	}

	public async Task<ShopItem<TItem>?> GetItem<TItem>(string name = "roles", byte index = 0)
	{
		if (m_shopItemCache.Exists(index.ToString(), name))
		{
			return (ShopItem<TItem>?) m_shopItemCache.Get(index.ToString(), name);
		}
		
		var shop = await GetShop<TItem>(name);
		
		if (shop == null || shop.Items.Count < index) return default;
		
		return shop.Items[index];
	}

	public async Task SetItem<TItem>(ShopItem<TItem> item, byte index = 0, string name = "roles")
	{
		if (item.Index != index) await RemoveItem<TItem>(item.Index, name);
		
		var shop = await GetShop<TItem>(name);
		if (shop == null) return;
		if (shop.Items.Capacity < index)
		{
			var list = new List<ShopItem<TItem>>(index + 1);
			list.AddRange(shop.Items);
			shop.Items = list;
		}
		shop.Items.Insert(index, item);

		m_shopItemCache.Put(item.Index.ToString(), name);
		
		await SetShop(shop);
	}
	
	public async Task RemoveItem<TItem>(byte index = 0, string name = "roles")
	{
		var shop = (await GetShop<TItem>(name))!;
		var item = (await GetItem<TItem>(name, index))!;
		
		shop.Items.Remove(item);
		m_shopItemCache.Remove(index.ToString(), name);
		
		await SetShop(shop);
	}

	public async Task SetProfiles(Profile[] profiles)
	{
		foreach (var profile in profiles)
		{
			m_profileCache.Put(profile.Id.ToString(), profile);
			
			if (await m_profiles.FindOneAndReplaceAsync(profile1 => profile1.Id == profile.Id, profile) == null)
			{
				await m_profiles.InsertOneAsync(profile);
			}
		}
	}
	public async Task SetProfiles(Profile profile)
	{
		m_profileCache.Put(profile.Id.ToString(), profile);
		
		if (await m_profiles.FindOneAndReplaceAsync(profile1 => profile1.Id == profile.Id, profile) != null)
		{
			return;
		}
		
		await m_profiles.InsertOneAsync(profile);
	}

	public async Task SetShop<TItem>(Shop<TItem> shop)
	{
		m_shopCache.Put(shop.Name, shop, "shop");
		
		var collection = m_database.GetCollection<Shop<TItem>>("shop");
		if (await collection.FindOneAndReplaceAsync(shop1 => shop1.Name == shop.Name, shop) != null)
		{
			return;
		}
		
		await collection.InsertOneAsync(shop);
	}
}