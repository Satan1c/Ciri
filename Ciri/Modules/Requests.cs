using Ciri.Models.Modals.Requests;
using Ciri.Modules.Configs.Requests;
using Ciri.Modules.Utils;
using Discord;
using Discord.Interactions;

namespace Ciri.Modules;

[Group("requests", "webhooks category")]
[EnabledInDm(false)]
public class Requests : InteractionModuleBase<SocketInteractionContext>
{
	[ComponentInteraction("staff_request_select", true)]
	public Task StaffSelect(string[] roles)
	{
		var properties = roles.First() switch
		{
			"moder" => RequestsConfig.ModersRequestMessageProperties,
			"eventer" => RequestsConfig.EventersRequestMessageProperties,
			_ => throw new ArgumentOutOfRangeException()
		};
		return Context.Interaction.RespondAsync(
			properties.Content,
			properties.Embeds,
			embed: properties.Embed,
			components: properties.Components,
			ephemeral: true);
	}

	[MessageCommand("Update")]
	public Task Update(IMessage message)
	{
		return Context.Interaction.RespondWithModalAsync<UpdateModal>(
			$"update_webhook_{message.Channel.Id}_{message.Id}");
	}

	[SlashCommand("send", "sends information to channel")]
	public Task Send(ITextChannel channel)
	{
		return Context.Interaction.RespondWithModalAsync<SendModal>($"send_webhook_{channel.Id}");
	}

	[ComponentInteraction("moder_request", true)]
	public Task ModerRequest()
	{
		return Context.Interaction.RespondWithModalAsync<ModerRequestModal>("moder_modal");
	}

	[ComponentInteraction("eventer_request", true)]
	public Task EventerRequest()
	{
		return Context.Interaction.RespondWithModalAsync<EventerRequestModal>("eventer_modal");
	}

	[ModalInteraction("update_webhook_*_*", true)]
	public async Task UpdateWebhook(ITextChannel channel, ulong messageId, UpdateModal modal)
	{
		if (messageId == 0)
		{
			await SendWebhook(channel, modal);
			return;
		}

		var props = RequestsUtils.GetProperties(modal.Topic, modal.Data);

		await channel.ModifyMessageAsync(messageId, properties =>
		{
			properties.Content = props.Content;
			properties.Embed = props.Embed;
			properties.Embeds = props.Embeds;
			properties.Components = props.Components;
		});
		await Context.Interaction.RespondAsync("Done", ephemeral: true);
	}

	[ModalInteraction("send_webhook_*", true)]
	public async Task SendWebhook(ITextChannel channel, SendModal modal)
	{
		var props = RequestsUtils.GetProperties(modal.Topic, modal.Data);

		await channel.SendMessageAsync(
			props.Content,
			embed: props.Embed,
			embeds: props.Embeds,
			components: props.Components);
		await Context.Interaction.RespondAsync("Done", ephemeral: true);
	}

	[ModalInteraction("moder_modal", true)]
	public async Task ModerModal(ModerRequestModal modal)
	{
		await SendFormResultAsync(
			"Заявка на Модера",
			Context.Guild.GetTextChannel(RequestsConfig.ModersChannel),
			modal.GetFields());
	}

	[ModalInteraction("eventer_modal", true)]
	public async Task EventerModal(EventerRequestModal modal)
	{
		await SendFormResultAsync(
			"Заявка на Ивентера",
			Context.Guild.GetTextChannel(RequestsConfig.EventersChannel),
			modal.GetFields());
	}

	private async Task SendFormResultAsync(string topic, IMessageChannel channel, EmbedFieldBuilder[] fields)
	{
		await channel.SendMessageAsync($"<@{Context.Interaction.User.Id}>", embed: new EmbedBuilder()
			.WithTitle(topic)
			.WithFields(fields)
			.Build());

		await Context.Interaction.RespondAsync("Sent", ephemeral: true);
	}
}