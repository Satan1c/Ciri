using Discord;
using Discord.Interactions;

namespace Ciri.Models.Modals.Moderation;

public class BanModal : IModal
{
	[ModalTextInput("reason", TextInputStyle.Short, "Wrote a reason for ban")]
	public string Reason { get; set; }

	public string Title { get; set; } = "Ban";
}