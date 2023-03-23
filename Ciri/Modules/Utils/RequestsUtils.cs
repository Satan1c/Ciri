using Ciri.Modules.Configs.Requests;
using Discord;
using MessageProperties = Ciri.Models.MessageProperties;

namespace Ciri.Modules.Utils;

public static class RequestsUtils
{
	public static MessageProperties GetProperties(string topic, string data)
	{
		return topic switch
		{
			"cinema" => SendCinemaHook(data),
			"staff" => SendStaffRequestHook(data),
			_ => throw new ArgumentOutOfRangeException()
		};
	}
	
	public static MessageProperties SendStaffRequestHook(string data)
	{
		return RequestsConfig.RequestsMessageProperties;
	}
	
	public static MessageProperties SendCinemaHook(string data)
	{
		var titles = data.Split(":-:");
		
		return new MessageProperties()
		{
			Embed = new EmbedBuilder()
				.WithColor(3093046)
				.WithAuthor("Расписание")
				.WithFields(GetFields(titles))
				.Build()
		};
	}

	private static EmbedFieldBuilder[] GetFields(string[] raw)
	{
		var res = new EmbedFieldBuilder[raw.Length];
		
		for (byte i = 0; i < raw.Length; i++)
		{
			var data = raw[i].Split('\n').Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToArray();
			var name = RequestsConfig.Dates[i];
			var value = "\u200b";
			
			if (data.Length >= 2)
				value = $"[{data[0]}]({data[1]})";

			res[i] = new EmbedFieldBuilder().WithName(name).WithValue(value).WithIsInline(true);
		}

		return res;
	}
}