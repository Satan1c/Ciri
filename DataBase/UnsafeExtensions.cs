using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CacheManager.Core;
using DataBase.Models;
using MongoDB.Driver;

namespace DataBase;

public static class UnsafeExtensions
{
	public static async ValueTask<bool> HasDocument<TDocument, TKey>(this IMongoCollection<TDocument> collection,
		ICacheManager<TDocument> cacheManager,
		FilterDefinition<TDocument> filterDefinition,
		TKey id)
	{
		var itemId = id!.ToString();
		if (cacheManager.Exists(itemId))
			return true;

		var item = await collection.Find(filterDefinition).ToListAsync().ConfigureAwait(false);
		if (item.Count < 1) return false;

		cacheManager.Put(itemId, item[0]);
		return true;
	}

	public static async ValueTask InsertOrReplaceOne<TDocument>(this IMongoCollection<TDocument> collection,
		FilterDefinition<TDocument> filterDefinition,
		TDocument document)
	{
		var count = await collection.CountDocumentsAsync(filterDefinition).ConfigureAwait(false);
		
		if (count > 0)
			await collection.ReplaceOneAsync(filterDefinition, document).ConfigureAwait(false);
		else
			await collection.InsertOneAsync(document).ConfigureAwait(false);
	}

	public static async ValueTask SetDocument<TDocument>(this IMongoCollection<TDocument> collection,
		FilterDefinition<TDocument> filterDefinition,
		ICacheManager<TDocument> cacheManager,
		string key,
		TDocument document,
		TDocument before = default!)
	{
		if (document.AreSame(before)) return;

		cacheManager.Put(key, document);
		await collection.InsertOrReplaceOne(
			filterDefinition,
			document).ConfigureAwait(false);
	}

	public static bool AreSame<T>(this T left, T right)
	{
		return EqualityComparer<T>.Default.Equals(left, right);
	}

	public static Profile[] GetProfilesUnsafe(this ICacheManager<Profile> profileCache, ref ulong[] ids)
	{
		var result = new Profile[ids.Length];
		ref var startResult = ref MemoryMarshal.GetArrayDataReference(result);
		ref var start = ref MemoryMarshal.GetArrayDataReference(ids);
		ref var end = ref Unsafe.Add(ref start, ids.Length);

		while (Unsafe.IsAddressLessThan(ref start, ref end))
		{
			var id = start.ToString();
			if (profileCache.Exists(id))
				startResult = profileCache.Get(id);

			start = ref Unsafe.Add(ref start, 1);
			startResult = ref Unsafe.Add(ref startResult, 1);
		}

		return result.ToArray();
	}

	public static void GetProfilesUnsafe(this ICacheManager<Profile> profileCache,
		ref Profile[] profiles,
		ref List<Profile> fetched)
	{
		var result = new Profile[profiles.Length + fetched.Count];
		profiles.CopyTo(result, 0);
		var fetchedSpan = CollectionsMarshal.AsSpan(fetched);
		ref var start = ref MemoryMarshal.GetArrayDataReference(result);
		ref var end = ref Unsafe.Add(ref start, result.Length);

		start = ref Unsafe.Add(ref start, profiles.Length);
		var counter = 0;
		while (Unsafe.IsAddressLessThan(ref start, ref end))
		{
			var item = fetchedSpan[counter];
			profileCache.Put(item.Id.ToString(), item);
			start = item;
			start = ref Unsafe.Add(ref start, 1);
			counter++;
		}

		profiles = result;
	}

	public static void UpdateUnsafe(this Profile[] profiles,
		ref string oldId,
		ref ShopItem newItem)
	{
		ref var start = ref MemoryMarshal.GetArrayDataReference(profiles);
		ref var end = ref Unsafe.Add(ref start, profiles.Length);

		while (Unsafe.IsAddressLessThan(ref start, ref end))
		{
			start.Inventory.Remove(oldId);
			start = ref Unsafe.Add(ref start, 1);
		}

		if (newItem.AreSame(default)) return;

		var newId = $"roles_{newItem.Name}_{newItem.Index}";
		start = ref MemoryMarshal.GetArrayDataReference(profiles);
		end = ref Unsafe.Add(ref start, profiles.Length);

		while (Unsafe.IsAddressLessThan(ref start, ref end))
		{
			start.Inventory.AddLast(newId);
			start = ref Unsafe.Add(ref start, 1);
		}
	}
}