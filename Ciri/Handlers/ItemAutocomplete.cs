using DataBase;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;

namespace Ciri.Handlers;

public class ItemAutocomplete : AutocompleteHandler
{
	private static DataBaseProvider? s_dataBaseProvider = null;
	public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
		IInteractionContext context,
		IAutocompleteInteraction autocompleteInteraction,
		IParameterInfo parameter,
		IServiceProvider services)
	{
		s_dataBaseProvider ??= services.GetRequiredService<DataBaseProvider>();
			
		try
		{
			var items = (await s_dataBaseProvider.GetShop<ulong>())!.Items;
			var userInput = autocompleteInteraction.Data.Current.Value.ToString()!.Trim();
			var pick =
				(string.IsNullOrEmpty(userInput)
					? items
					: byte.TryParse(userInput, out _)
						? items.Where(x => x.Index.ToString().StartsWith(userInput))
						: items.Where(x => x.Name.ToString().StartsWith(userInput))).Take(5);

			return AutocompletionResult.FromSuccess(
				pick.Select(x => new AutocompleteResult(x.Name, x.Index.ToString())));
		}
		catch (Exception e)
		{
			return AutocompletionResult.FromError(e);
		}
	}
}