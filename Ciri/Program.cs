using Ciri.Handlers;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Extensions.Logging;
using ShikimoriSharp;
using ShikimoriSharp.Bases;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Logger = Microsoft.Extensions.Logging.Logger<Microsoft.Extensions.Logging.ILogger>;

await using var services = new ServiceCollection()
	.AddSingleton<Serilog.ILogger>(new LoggerConfiguration()
		.Enrich.FromLogContext()
		.MinimumLevel.Verbose()
		.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u4}]\t{Message:lj}\t{Exception}\n")
		.CreateLogger()
	)
	.AddSingleton<ILogger>(provider => new Logger(new SerilogLoggerFactory(provider.GetRequiredService<Serilog.ILogger>())))
	.AddSingleton(new DiscordSocketConfig
	{
		AlwaysDownloadUsers = false,
		AlwaysDownloadDefaultStickers = false,
		AlwaysResolveStickers = false,
		MessageCacheSize = 1,
		GatewayIntents = GatewayIntents.All,
		LogLevel = LogSeverity.Verbose
	})
	.AddSingleton(new InteractionServiceConfig
	{
		UseCompiledLambda = true,
		DefaultRunMode = RunMode.Async,
		LogLevel = LogSeverity.Verbose
	})
	.AddSingleton(new ClientSettings(
		"Geno",
		"mkGRM2ud5xmOqUl5bvZkUbFV-zqjQimkQ-W5hhPBFR0",
		"OlOUNsD14GN2TM6WHwaUaEuqrkFS7LGKJfwtHvyf6Ck"
		)
	)
	
	.AddSingleton<DiscordSocketClient>()
	.AddSingleton<InteractionService>()
	.AddSingleton<InteractionHandler>()
	.AddSingleton<ClientEvents>()
	.AddSingleton<GuildEvents>()
	
	.AddSingleton<ShikimoriClient>()
	
	.BuildServiceProvider();
	
var bot = services.GetRequiredService<DiscordSocketClient>();
services.GetRequiredService<ClientEvents>();

await bot.LoginAsync(TokenType.Bot, "Njk5NjczMzUzMDE1MDAxMTE4.GKFBr3.w0mntSHnjL4RS5oZvRQ4FSSpk3xB5Ly3O-84QI", false);
await bot.StartAsync();
await Task.Delay(Timeout.Infinite);