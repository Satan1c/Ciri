using Discord;
using Discord.Interactions;

namespace Ciri.Models.Modals.Requests;

public class SendModal : IModal
{
	[ModalTextInput("topic")] public string Topic { get; set; }

	[ModalTextInput("data", TextInputStyle.Paragraph)]
	public string Data { get; set; }

	public string Title { get; } = "Send message";
}