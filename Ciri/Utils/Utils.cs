using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace Ciri.Utils;

public static class Utils
{
	public static IDictionary<string, string> GetEnv()
	{
		return ((Hashtable)Environment.GetEnvironmentVariables()).Cast<DictionaryEntry>()
			.ToDictionary(
				kvp
					=> (string)kvp.Key, kvp => (string)kvp.Value!);
	}

	public static IServiceCollection AddCronScheduler(this IServiceCollection collection)
	{
		collection.AddScheduler();
		return collection;
	}
}