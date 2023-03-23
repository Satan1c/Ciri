using Discord;
using Discord.Interactions;

namespace Ciri.Models.Modals.Requests;

public class EventerRequestModal : RequestModal
{
	public new string Title => base.Title + " на Ивентера";
	
	[InputLabel("Какие ивенты вы умеете проводить")]
	[ModalTextInput("experience", placeholder: "Например: алиас/мафия/джебокс")]
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