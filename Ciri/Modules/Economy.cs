using System.Runtime.CompilerServices;
using System.Text;
using Ciri.Handlers;
using Ciri.Modules.Configs;
using Ciri.Modules.Utils;
using DataBase;
using DataBase.Models;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MongoDB.Bson.Serialization.Serializers;

namespace Ciri.Modules;

[Group("economy", "economy commands")]
[EnabledInDm(false)]
public class Economy : InteractionModuleBase<SocketInteractionContext>
{
	private readonly DataBaseProvider m_dataBaseProvider;
	private readonly DiscordSocketClient m_client;
	private readonly CronTimer m_timer = new("0 0 * * 0", "UTC");
	private readonly ITextChannel m_channel;

	public Economy(DataBaseProvider dataBaseProvider, GuildEvents guildEvents, DiscordSocketClient client)
	{
		m_channel = client.GetGuild(542005378049638400).GetTextChannel(684011228531654658);
		m_dataBaseProvider = dataBaseProvider;
		m_client = client;
		m_timer.OnOccurence += async (_, _) =>
		{
			var description = new StringBuilder();
			foreach (var (role, profit) in guildEvents.Profit)
			{
				var profiles = await m_dataBaseProvider.GetProfiles(profit.Members);
				Array.ForEach(profiles, profile => profile.Hearts += profit.Hearts);
				
				await m_dataBaseProvider.SetProfiles(profiles);

				description.Add(
					$"<&{role}> получила зарплату в количестве **__{profit.Hearts}__**{EmojiConfig.HeartVal}");
			}

			await m_channel.SendMessageAsync(embed: new EmbedBuilder()
				.WithDescription(description.ToString())
				.WithColor(3093046).Build());
		};
		m_timer.Start();
	}
	
	[SlashCommand("shop", "shop command")]
	public async Task Shop()
	{
		var shop = await m_dataBaseProvider.GetShop<ulong>();
		if (shop == null)
		{
			await Context.Interaction.RespondAsync("Shop not found", ephemeral: true);
			return;
		}
		
		var profile = await m_dataBaseProvider.GetProfiles(Context.User.Id);
		var embeds = new LinkedList<EmbedBuilder>();

		for (var i = 0; i < shop.Items.Count; i += 5)
		{
			var embed = new EmbedBuilder()
				.WithTitle("Магазин")
				.WithColor(3093046);
			
			foreach (var shopItem in shop.Items.Take(i..(i + 5)))
			{
				embed.AddField($"[{shopItem.Index + 1}] {shopItem.Name}", $"{shop.GetCost(shopItem).ToString()}{EmojiConfig.HeartVal}");
			}
			
			embeds.AddLast(embed);
		}

		var max = embeds.Count;
		var first = embeds.First!.Value;
		var ids = new []
		{
			"shop_left",
			"shop_close",
			"shop_right",
		};
		
		var items = shop.Items.Select(x => shop.GetCost(x)).ToArray();
		var currentItems = items.Take(..5).ToArray();
		
		var timeout = TimeSpan.FromMinutes(2);
		var components = new ComponentBuilder()
			.SetShopControls(1, currentItems, profile.Hearts, ids)
			.Build();
		var closeAt = DateTimeOffset.UtcNow.Add(timeout.Subtract(TimeSpan.FromSeconds(3)));

		await Context.Interaction.RespondAsync(embed: first.SetPage(1, max, closeAt).Build(), components: components, ephemeral: true);

		
		var currentPage = 1;
		var current = first;
		while (DateTimeOffset.UtcNow < closeAt)
		{
			
			var interaction = await InteractionUtility.WaitForInteractionAsync(m_client, timeout, interaction =>
			{
				var componentInteraction = interaction as IComponentInteraction;
				return interaction.Type == InteractionType.MessageComponent && componentInteraction != null &&
				       (ids.Contains(componentInteraction.Data.CustomId) || componentInteraction.Data.CustomId.StartsWith("buy_"));
			});
			var componentInteraction = (interaction as IComponentInteraction)!;
			closeAt = DateTimeOffset.UtcNow.Add(timeout);
			
			await componentInteraction.DeferAsync(true);
			
			if (componentInteraction.Data.CustomId == ids[1])
			{
				break;
			}

			if (componentInteraction.Data.CustomId == ids[0])
			{
				current = current.MoveLeft(ref currentPage, ref max, ref embeds);
				currentItems = await interaction.UpdateShop(
					current, currentPage, max,
					closeAt, items,
					ids, profile, currentItems);
			}
			else if (componentInteraction.Data.CustomId == ids[2])
			{
				current = current.MoveRight(ref currentPage, ref max, ref embeds);
				currentItems = await interaction.UpdateShop(
					current, currentPage, max,
					closeAt, items,
					ids, profile, currentItems);
			}
			else if (componentInteraction.Data.CustomId.StartsWith("buy_"))
			{
				var index = byte.Parse(componentInteraction.Data.CustomId.Split("_")[^1]);
				var item = await m_dataBaseProvider.GetItem<ulong>(index: index);
				
				if (item == null)
				{
					await Context.Interaction.FollowupAsync("Item not found", ephemeral: true);
					return;
				}
				
				profile.Hearts -= shop.GetCost(item);
				await m_dataBaseProvider.SetProfiles(profile);
				await Context.Interaction.FollowupAsync($"{item.Name} bought", ephemeral: true);
				
				currentItems = await interaction.UpdateShop(
					current, currentPage, max,
					closeAt, items,
					ids, profile, currentItems);
			}
		}
		
		await Context.Interaction.CloseShop(currentPage, current.Fields.Count, ids);
	}
}