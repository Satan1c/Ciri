using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Ciri.Handlers;

public class ClientEvents
{
	private static ILogger? s_logger;
	private readonly DiscordSocketClient m_client;
	private readonly GuildEvents m_guildEvents;
	private readonly InteractionHandler m_interactionHandler;

	public ClientEvents(IServiceProvider serviceProvider)
	{
		m_client = serviceProvider.GetRequiredService<DiscordSocketClient>();
		m_interactionHandler = serviceProvider.GetRequiredService<InteractionHandler>();
		m_guildEvents = serviceProvider.GetRequiredService<GuildEvents>();
		s_logger = serviceProvider.GetRequiredService<ILogger>();

		m_client.Ready += OnReady;
		m_client.Log += OnLog;
	}

	public static Task OnLog(LogMessage message)
	{
		s_logger?.Write(
			SeverityToLevel(message.Severity),
			message.Exception,
			"[{Source}]\t{Message} {Trace} {InnerTrace}",
			message.Source,
			message.Message,
			message.Exception != null ? $"\n{message.Exception.StackTrace?.Replace("\n", "\n\t\t\t")}" : "",
			message.Exception is { StackTrace: { } }
				? $"\n{message.Exception.StackTrace.Replace("\n", "\n\t\t\t")}"
				: "");

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

		await m_guildEvents.Init().ConfigureAwait(false);
		await m_interactionHandler.Init().ConfigureAwait(false);

		await OnLog(new LogMessage(LogSeverity.Verbose, nameof(OnReady), "end of ready event"));
	}
}