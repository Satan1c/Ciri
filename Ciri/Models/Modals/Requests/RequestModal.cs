using Discord;
using Discord.Interactions;

namespace Ciri.Models.Modals.Requests;

public class RequestModal : IModal
{
	public string Title => "Заявка";
	
	[InputLabel("Ваше имя и возраст")]
	[ModalTextInput("name", placeholder: "Например: Вася 18 лет")]
	public string NameAndAge { get; set; }

	[InputLabel("Ваш часовой пояс")]
	[ModalTextInput("time_zone", minLength: 0, placeholder: "Например: UTC+3")]
	public string? TimeZone { get; set; } = null;
	
	[InputLabel("Сколько времени готовы уделять проекту")]
	[ModalTextInput("free_time", placeholder: "Например: 6ч 10:00-16:00")]
	public string FreeTime { get; set; }

	[InputLabel("Почему мы должны взять именно вас")]
	[ModalTextInput("reason", TextInputStyle.Paragraph)]
	public string Reason { get; set; }
	
	internal EmbedFieldBuilder[] GetFields()
	{
		return new []
		{
			new EmbedFieldBuilder().WithName("Имя и возрас:").WithValue(NameAndAge),
			new EmbedFieldBuilder().WithName("Часовой пояс:").WithValue(TimeZone ?? "не указан"),
			new EmbedFieldBuilder().WithName("Свободное время:").WithValue(FreeTime),
			new EmbedFieldBuilder().WithName("Причина:").WithValue(Reason)
		};
	}
}