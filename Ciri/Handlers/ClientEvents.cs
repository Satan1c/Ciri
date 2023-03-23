using Discord;
using Discord.WebSocket;
using Serilog;
using Serilog.Events;

namespace Ciri.Handlers;

public class ClientEvents
{
	private readonly DiscordSocketClient m_client;
	private readonly InteractionHandler m_interactionHandler;
	private readonly GuildEvents m_guildEvents;
	private static ILogger? m_logger;

	public ClientEvents(DiscordSocketClient client, InteractionHandler interactionHandler, GuildEvents guildEvents, ILogger logger)
	{
		m_client = client;
		m_interactionHandler = interactionHandler;
		m_guildEvents = guildEvents;
		m_logger = logger;

		m_client.Ready += OnReady;
		m_client.Log += OnLog;
	}

	public static Task OnLog(LogMessage message)
	{
		m_logger?.Write(SeverityToLevel(
				message.Severity),
			message.Exception,
			"[{Source}]\t{Message}",
			message.Source,
			message.Message);
		
		return Task.CompletedTask;
	}

	private static LogEventLevel SeverityToLevel(LogSeverity severity)
	{
		return severity switch
		{
			LogSeverity.Critical => LogEventLevel.Fatal,
			LogSeverity.Error => LogEventLevel.Error,
			LogSeverity.Warning => LogEventLevel.Warning,
			LogSeverity.Info => LogEventLevel.Information,
			LogSeverity.Verbose => LogEventLevel.Verbose,
			LogSeverity.Debug => LogEventLevel.Debug,
			_ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
		};
	}

	public async Task OnReady()
	{
		m_client.Ready -= OnReady;
		
		await m_interactionHandler.Init();
		await m_guildEvents.Init();

		await OnLog(new LogMessage(LogSeverity.Verbose, nameof(OnReady), "end of ready event"));
	}
}