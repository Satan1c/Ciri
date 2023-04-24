using CacheManager.Core;
using DataBase.Models;
using MongoDB.Driver;

namespace DataBase;

public class DataBaseProvider
{
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

		var database = client.GetDatabase("main");
		m_profiles = database.GetCollection<Profile>("profiles");
		m_shop = database.GetCollection<Shop>("shop");
	}

	public async ValueTask<bool> HasProfile(ulong id)
	{
		return await m_profiles.HasDocument(
			m_profileCache,
			Builders<Profile>.Filter.Eq(x => x.Id, id),
			id).ConfigureAwait(false);
	}

	public async ValueTask<bool> HasShop()
	{
		return await m_shop.HasDocument(
			m_shopCache,
			Builders<Shop>.Filter.Eq(x => x.Name, "roles"),
			"roles").ConfigureAwait(false);
	}

	public async ValueTask<Profile> GetProfiles(ulong id)
	{
		Profile item;
		var itemId = id.ToString();
		if (await HasProfile(id).ConfigureAwait(false))
		{
			item = m_profileCache.Get(itemId);
		}
		else
		{
			item = new Profile
			{
				Id = id
			};
      
			m_profileCache.Put(itemId, item);
		}

		return item;
	}

	public async ValueTask<Profile[]> GetProfiles(ulong[] ids)
	{
		var tasks = ids.Select(async arg => await GetProfiles(arg).ConfigureAwait(false)).ToArray();
		var res = await Task.WhenAll(tasks).ConfigureAwait(false);
		
		return res;
	}

	public async ValueTask<Shop> GetShop()
	{
		Shop item;
		const string itemId = "roles";
		if (await HasShop().ConfigureAwait(false))
		{
			item = m_shopCache.Get(itemId);
		}
		else
		{
			item = new Shop();
			m_shopCache.Put(itemId, item);
		}

		return item;
	}

	public async ValueTask<ShopItem> GetItem(byte index = 0)
	{
		var id = index.ToString();
		if (m_shopItemCache.Exists(id))
			return m_shopItemCache.Get(id);

		var shop = await GetShop().ConfigureAwait(false);
		var item = shop.Items.Count < index + 1
			? new ShopItem()
			: shop.Items[index];

		m_shopItemCache.Put(id, item);

		return item;
	}

	public async ValueTask SetItem(ShopItem item, byte index = 0)
	{
		var shop = await GetShop().ConfigureAwait(false);
		var oldShop = shop.GetCopy();
		var oldItem = await GetItem(item.Index).ConfigureAwait(false);
		if (!oldItem.AreSame(default) && (oldItem.Index != index || oldItem.Name != item.Name))
			await UpdateInventories(oldItem, item).ConfigureAwait(false);

		await RemoveItem(item.Index, false).ConfigureAwait(false);

		if (shop.Items.Capacity < index + 1)
		{
			var list = new List<ShopItem>(index + 2);
			list.AddRange(shop.Items);
			shop.Items = list;
		}

		shop.Items.Insert(index, item);
		m_shopItemCache.Put(index.ToString(), item);

		await SetShop(shop, oldShop).ConfigureAwait(false);
	}

	public async ValueTask RemoveItem(byte index = 0, bool remove = true)
	{
		var shop = await GetShop().ConfigureAwait(false);
		var oldShop = shop.GetCopy();
		var item = (await GetItem(index).ConfigureAwait(false))!;

		if (remove)
			await UpdateInventories(item).ConfigureAwait(false);

		shop.Items.Remove(item);
		m_shopItemCache.Remove(index.ToString());
		
		await SetShop(shop, oldShop).ConfigureAwait(false);
	}

	public async ValueTask UpdateInventories(ShopItem oldItem, ShopItem newItem = default)
	{
		var oldId = $"roles_{oldItem.Name}_{oldItem.Index}";
		var profiles = (await m_profiles.Find(p => p.Inventory.Contains(oldId)).ToListAsync().ConfigureAwait(false)).ToArray();

		profiles.UpdateUnsafe(ref oldId, ref newItem);

		await SetProfiles(profiles).ConfigureAwait(false);
	}

	public async ValueTask SetProfiles(Profile[] profiles)
	{
		var tasks = profiles.Select(async profile => await SetProfiles(profile).ConfigureAwait(false));
		await Task.WhenAll(tasks).ConfigureAwait(false);
	}

	public async ValueTask SetProfiles(Profile profile, Profile before = default)
	{
		if (profile.AreSame(default)) return;
		if (before.AreSame(default)) before = await GetProfiles(profile.Id).ConfigureAwait(false);
		
		await m_profiles.SetDocument(
				Builders<Profile>.Filter.Eq(x => x.Id, profile.Id),
				m_profileCache,
				profile.Id.ToString(),
				profile,
				before)
			.ConfigureAwait(false);
	}

	public async ValueTask SetShop(Shop shop, Shop before = default)
	{
		if (before.AreSame(default)) before = await GetShop().ConfigureAwait(false);
		
		await m_shop.SetDocument(
				Builders<Shop>.Filter.Eq(x => x.Name, "roles"),
				m_shopCache,
				"roles",
				shop,
				before)
			.ConfigureAwait(false);
	}
}