using System.Text;
using Ciri.Handlers;
using Ciri.Utils;
using DataBase;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using Serilog;
using ShikimoriSharp;
using ShikimoriSharp.Bases;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using JsonLocalizationManager = Localization.JsonLocalizationManager;
using Logger = Microsoft.Extensions.Logging.Logger<Microsoft.Extensions.Logging.ILogger>;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

var env = Utils.GetEnv();
var locals = Path.GetFullPath("../../", AppDomain.CurrentDomain.BaseDirectory) + "Localizations";
var jsons = locals + "/json";
var csv = locals + "/csv";

await using var services = new ServiceCollection()
	.AddSingleton<Serilog.ILogger>(new LoggerConfiguration()
		.Enrich.FromLogContext()
		.MinimumLevel.Verbose()
		.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u4}]\t{Message:lj}\t{Exception}\n")
		.CreateLogger()
	)
	.AddSingleton<ILogger>(provider => NullLogger.Instance)
	.AddSingleton(new DiscordSocketConfig
	{
		AlwaysDownloadUsers = false,
		AlwaysDownloadDefaultStickers = false,
		AlwaysResolveStickers = false,
		MessageCacheSize = 1,
		GatewayIntents = GatewayIntents.GuildMembers
		                 | GatewayIntents.GuildMessages
		                 | GatewayIntents.Guilds
		                 | GatewayIntents.GuildVoiceStates,
		LogLevel = LogSeverity.Info
	})
	.AddSingleton(new InteractionServiceConfig
	{
		UseCompiledLambda = true,
		DefaultRunMode = RunMode.Async,
		LogLevel = LogSeverity.Verbose,
		LocalizationManager = new JsonLocalizationManager(jsons)
	})
	.AddSingleton(new ClientSettings(
			env["ShikimoriClientName"],
			env["ShikimoriClientId"],
			env["ShikimoriClientSecret"]
		)
	)
	.AddSingleton(MongoClientSettings.FromConnectionString(env["Mongo"]))
	.AddSingleton(new LocalizationManager(csv))
	.AddSingleton<IMongoClient, MongoClient>()
	.AddSingleton<DataBaseProvider, DataBaseProvider>()
	.AddSingleton<DiscordSocketClient>()
	.AddSingleton<InteractionService>()
	.AddSingleton<InteractionHandler>()
	.AddSingleton<ClientEvents>()
	.AddSingleton<GuildEvents>()
	.AddSingleton<ShikimoriClient>()
	.BuildServiceProvider();

var bot = services.GetRequiredService<DiscordSocketClient>();
services.GetRequiredService<ClientEvents>();

await bot.LoginAsync(TokenType.Bot, env["Token"], false).ConfigureAwait(false);
await bot.StartAsync().ConfigureAwait(false);
await Task.Delay(Timeout.Infinite);