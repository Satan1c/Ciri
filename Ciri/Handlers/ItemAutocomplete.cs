using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DataBase;
using DataBase.Models;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;

namespace Ciri.Handlers;

public class ItemAutocomplete : AutocompleteHandler
{
	private static DataBaseProvider? s_dataBaseProvider;

	public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
		IInteractionContext context,
		IAutocompleteInteraction autocompleteInteraction,
		IParameterInfo parameter,
		IServiceProvider services)
	{
		s_dataBaseProvider ??= services.GetRequiredService<DataBaseProvider>();

		try
		{
			var items = (await s_dataBaseProvider.GetShop())!.Items;
			var userInput = autocompleteInteraction.Data.Current.Value.ToString()!.Trim();

			return AutocompletionResult.FromSuccess(items.GetAutocompleteResults(ref userInput));
		}
		catch (Exception e)
		{
			return AutocompletionResult.FromError(e);
		}
	}
}

public static class UnsafeExtensions
{
	public static IEnumerable<AutocompleteResult> GetAutocompleteResults(this List<ShopItem> shopItems,
		ref string userInput)
	{
		var itemsRaw = shopItems.ToArray().AsSpan();
		var items = new AutocompleteResult[5];

		ref var start = ref MemoryMarshal.GetReference(items.AsSpan());
		ref var end = ref Unsafe.Add(ref start, items.Length);

		ref var startItems = ref MemoryMarshal.GetReference(itemsRaw);
		ref var endItems = ref Unsafe.Add(ref startItems, itemsRaw.Length);

		//var count = 0;
		if (string.IsNullOrEmpty(userInput))
			while (Unsafe.IsAddressLessThan(ref start, ref end))
			{
				start = new AutocompleteResult(startItems.Name, startItems.Index.ToString());
				start = ref Unsafe.Add(ref start, 1);
				
				startItems = ref Unsafe.Add(ref startItems, 1);
				//count++;
			}
		else if (byte.TryParse(userInput, out _))
			while (Unsafe.IsAddressLessThan(ref startItems, ref endItems) &&
			       Unsafe.IsAddressLessThan(ref start, ref end))
			{
				if (startItems.Index.ToString().StartsWith(userInput))
				{
					start = new AutocompleteResult(startItems.Name, startItems.Index.ToString());
					start = ref Unsafe.Add(ref start, 1);
				}
				
				startItems = ref Unsafe.Add(ref startItems, 1);
				//count++;
			}
		else
			while (Unsafe.IsAddressLessThan(ref startItems, ref endItems) &&
			       Unsafe.IsAddressLessThan(ref start, ref end))
			{
				if (!startItems.Name.StartsWith(userInput))
				{
					start = new AutocompleteResult(startItems.Name, startItems.Index.ToString());
					start = ref Unsafe.Add(ref start, 1);
				}
				
				startItems = ref Unsafe.Add(ref startItems, 1);
				//count++;
			}

		return items;
	}
}