using Discord;
using Discord.Interactions;

namespace Ciri.Models.Modals.Moderation;

public class BanModal : IModal
{
	public string Title { get; set; } = "Ban";
	
	[ModalTextInput("reason", TextInputStyle.Short, "Wrote a reason for ban")]
	public string Reason { get; set; }
}