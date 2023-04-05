using Discord;
using Discord.Interactions;

namespace Ciri.Models.Modals.Requests;

public class ModerRequestModal : RequestModal
{
	public new string Title => base.Title + " на Модератора";

	[InputLabel("Есть ли у вас опыт работы модером")]
	[ModalTextInput("experience", placeholder: "есть/нет")]
	public string Experience { get; set; }

	internal new EmbedFieldBuilder[] GetFields()
	{
		var fields = base.GetFields();
		var result = new EmbedFieldBuilder[fields.Length + 1];

		fields.CopyTo(result, 0);
		result[^1] = new EmbedFieldBuilder().WithName("Опыт:").WithValue(Experience);

		return result;
	}
}