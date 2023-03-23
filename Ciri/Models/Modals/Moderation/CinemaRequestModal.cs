using Discord;
using Discord.Interactions;

namespace Ciri.Models.Modals.Moderation;

public class CinemaRequestModal : IModal
{
	public string Title => "Запрос на просмотр";
	
	[InputLabel("Напишите название аниме/фильма")]
	[ModalTextInput("name", TextInputStyle.Short, "Наименование аниме/фильма в оригинале или на английском, например: Kimi no Na wa.")]
	public string Name { get; set; }
	
	[InputLabel("Укажите ссылку на аниме/фильм")]
	[ModalTextInput("url", TextInputStyle.Short, "Укажите ссылку на аниме/фильм с Shikimori или Wikipedia")]
	public string Url { get; set; }
	
	[InputLabel("Напишите описание аниме/фильма")]
	[ModalTextInput("description", TextInputStyle.Paragraph, "Укажите краткое описание аниме/фильма", 1, 3000)]
	public string Description { get; set; }
}