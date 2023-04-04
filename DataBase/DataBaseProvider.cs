using CacheManager.Core;
using DataBase.Models;
using MongoDB.Driver;

namespace DataBase;

public class DataBaseProvider
{
	private readonly IMongoDatabase m_database;
	private readonly ICacheManager<Profile> m_profileCache;
	private readonly IMongoCollection<Profile> m_profiles;
	private readonly IMongoCollection<Shop> m_shop;
	
	private readonly ICacheManager<Shop> m_shopCache;
	private readonly ICacheManager<ShopItem> m_shopItemCache;

	public DataBaseProvider(MongoClient client) : this(client as IMongoClient)
	{
	}

	public DataBaseProvider(IMongoClient client)
	{
		m_profileCache = CacheFactory.Build<Profile>(part =>
			part.WithMicrosoftMemoryCacheHandle()
				.WithExpiration(ExpirationMode.Sliding, TimeSpan.FromMinutes(60)));

		m_shopCache = CacheFactory.Build<Shop>(settings =>
			settings.WithMicrosoftMemoryCacheHandle()
				.WithExpiration(ExpirationMode.Sliding, TimeSpan.FromDays(7)));

		m_shopItemCache = CacheFactory.Build<ShopItem>(part =>
			part.WithMicrosoftMemoryCacheHandle()
				.WithExpiration(ExpirationMode.Sliding, TimeSpan.FromDays(1)));

		m_database = client.GetDatabase("main");
		m_profiles = m_database.GetCollection<Profile>("profiles");
		m_shop = m_database.GetCollection<Shop>("shop");
	}

	public async Task SyncCache()
	{
		var filter = Builders<Profile>.Filter.Empty;
		var options = new FindOptions<Profile> { BatchSize = 10 };
		var cursor = await m_profiles.FindAsync(filter, options);
		while (await cursor.MoveNextAsync())
			foreach (var profile in cursor.Current)
				m_profileCache.Put(profile.Id.ToString(), profile);
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
		if (m_profileCache.Exists(profileId)) return m_profileCache.Get(profileId);

		var filter = await m_profiles.Find(profile1 => profile1.Id == id).FirstOrDefaultAsync();
		if (filter == null) return Profile.GetDefault(id);

		m_profileCache.Put(profileId, filter);

		return filter;
	}

	public async Task<Profile[]> GetProfiles(ulong[] ids)
	{
		var profiles = m_profileCache.GetProfilesUnsafe(ref ids);
		if (profiles.Length == ids.Length) return profiles;

		var filter = await m_profiles
			.Find(profile1 => ids.Contains(profile1.Id) && !profiles.Contains(profile1))
			.ToListAsync();

		m_profileCache.GetProfilesUnsafe(ref profiles, ref filter);

		return profiles;
	}

	public async Task<Shop?> GetShop()
	{
		if (m_shopCache.Exists("shop")) return (Shop?)m_shopCache.Get("shop");

		var filter = await m_shop.Find(x => x.Name == "shop").FirstOrDefaultAsync();

		if (filter == null) return default;

		m_shopCache.Put("shop", filter);

		return filter;
	}

	public async Task<ShopItem?> GetItem( byte index = 0)
	{
		if (m_shopItemCache.Exists(index.ToString()))
			return m_shopItemCache.Get(index.ToString());

		var shop = await GetShop();

		if (shop == null || shop.Items.Count < index + 1) return default;

		return shop.Items[index];
	}

	public async Task SetItem(ShopItem item, byte index = 0)
	{
		var shop = await GetShop();
		if (shop == null) return;

		var oldItem = await GetItem(item.Index);
		if (oldItem != null && (oldItem.Index != index || oldItem.Name != item.Name))
			await UpdateInventories(oldItem, item);

		if (item.Index != index) await RemoveItem(item.Index, false);


		if (shop.Items.Capacity < index)
		{
			var list = new List<ShopItem>(index + 1);
			list.AddRange(shop.Items);
			shop.Items = list;
		}

		shop.Items.Insert(index, item);

		m_shopItemCache.Put(item.Index.ToString(), item);

		await SetShop(shop);
	}

	public async Task RemoveItem(byte index = 0, bool remove = true)
	{
		var shop = (await GetShop())!;
		var item = (await GetItem(index))!;

		if (remove) await UpdateInventories(item);

		shop.Items.Remove(item);
		m_shopItemCache.Remove(index.ToString());
		await SetShop(shop);
	}

	public async Task UpdateInventories(ShopItem oldItem, ShopItem? newItem = null)
	{
		var oldId = $"shop_{oldItem.Name}_{oldItem.Index}";
		var profiles = (await m_profiles.Find(p => p.Inventory.Contains(oldId)).ToListAsync()).ToArray();
		profiles.UpdateUnsafe(ref oldId, ref newItem);
		await SetProfiles(profiles);
	}

	public async Task SetProfiles(Profile[] profiles)
	{
		foreach (var profile in profiles)
		{
			m_profileCache.Put(profile.Id.ToString(), profile);

			if (await m_profiles.FindOneAndReplaceAsync(profile1 => profile1.Id == profile.Id, profile) == null)
				await m_profiles.InsertOneAsync(profile);
		}
	}

	public async Task SetProfiles(Profile profile)
	{
		m_profileCache.Put(profile.Id.ToString(), profile);
		if (await m_profiles.FindOneAndReplaceAsync(profile1 => profile1.Id == profile.Id, profile) != null) return;

		await m_profiles.InsertOneAsync(profile);
	}

	public async Task SetShop(Shop shop)
	{
		m_shopCache.Put("shop", shop);

		if (await m_shop.FindOneAndReplaceAsync(shop1 => shop1.Name == "shop", shop) != null) return;

		await m_shop.InsertOneAsync(shop);
	}
}