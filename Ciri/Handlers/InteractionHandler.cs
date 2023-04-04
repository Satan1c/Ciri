using System.Reflection;
using Discord.Extensions.Interactions;
using Discord.Interactions;
using Discord.WebSocket;
using Serilog;

namespace Ciri.Handlers;

public class InteractionHandler
{
	private readonly DiscordSocketClient m_client;
	private readonly InteractionService m_service;
	private readonly IServiceProvider m_serviceProvider;

	public InteractionHandler(DiscordSocketClient client,
		InteractionService service,
		IServiceProvider serviceProvider,
		ILogger logger)
	{
		m_client = client;
		m_service = service;
		m_serviceProvider = serviceProvider;
	}

	public async Task Init()
	{
		m_service.AddTypeConverter<ulong>(new UlongTypeConverter());

		await m_service.AddModulesAsync(Assembly.GetEntryAssembly(), m_serviceProvider);
		await m_service.RegisterCommandsGloballyAsync();

		m_client.InteractionCreated += InteractionCreate;
		//m_service.InteractionExecuted += InteractionExecuted;
		m_service.Log += ClientEvents.OnLog;
	}

	/*private static Task InteractionExecuted(ICommandInfo command, IInteractionContext ctx, IResult result)
	{
		if (result.ErrorReason is null) return Task.CompletedTask;

		return Task.CompletedTask;
	}*/

	public Task InteractionCreate(SocketInteraction interaction)
	{
		var ctx = new SocketInteractionContext(m_client, interaction);
		return m_service.ExecuteCommandAsync(ctx, m_serviceProvider);
	}
}