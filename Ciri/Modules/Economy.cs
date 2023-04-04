using System.Text;
using Ciri.Handlers;
using Ciri.Modules.Configs;
using Ciri.Modules.Utils;
using DataBase;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Ciri.Modules;

[Group("economy", "economy commands")]
[EnabledInDm(false)]
public class Economy : InteractionModuleBase<SocketInteractionContext>
{
	public static DataBaseProvider m_dataBaseProvider;
	private readonly DiscordSocketClient m_client;
	private readonly CronTimer m_timer = new("0 0 * * 0", "UTC");

	public Economy(DataBaseProvider dataBaseProvider, GuildEvents guildEvents, DiscordSocketClient client)
	{
		ITextChannel channel = client.GetGuild(542005378049638400).GetTextChannel(684011228531654658);
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

			await channel.SendMessageAsync(embed: new EmbedBuilder()
				.WithDescription(description.ToString())
				.WithColor(3093046).Build());
		};
		m_timer.Start();
	}

	[SlashCommand("give", "give hearts to user")]
	public async Task Give(IUser user, uint amount)
	{
		if (user.IsBot)
		{
			await RespondAsync("You can't give hearts to bot", ephemeral: true);
			return;
		}

		var myProfile = await m_dataBaseProvider.GetProfiles(Context.User.Id);
		if (myProfile.Hearts < amount)
		{
			await RespondAsync("You don't have enough hearts", ephemeral: true);
			return;
		}

		var userProfile = await m_dataBaseProvider.GetProfiles(user.Id);
		myProfile.Hearts -= amount;
		userProfile.Hearts += amount;

		await m_dataBaseProvider.SetProfiles(new[] { myProfile, userProfile });
		await RespondAsync($"You gave {user.Mention} {amount.ToString()}{EmojiConfig.HeartVal}", ephemeral: true);
	}

	[SlashCommand("shop", "shop command")]
	public async Task Shop()
	{
		var shop = await m_dataBaseProvider.GetShop();
		if (shop == null)
		{
			await Context.Interaction.RespondAsync("Shop not found", ephemeral: true);
			return;
		}

		var profile = await m_dataBaseProvider.GetProfiles(Context.User.Id);
		var embeds = new LinkedList<EmbedBuilder>();
		shop.GenerateEmbeds(ref embeds);

		var max = embeds.Count;
		var first = embeds.First!.Value;
		var ids = new[]
		{
			"shop_left",
			"shop_close",
			"shop_right"
		};

		var timeout = TimeSpan.FromMinutes(2);
		var components = new ComponentBuilder()
			.SetShopControls(1, shop, profile, ids)
			.Build();
		var closeAt = DateTimeOffset.UtcNow.Add(timeout.Subtract(TimeSpan.FromSeconds(3)));

		await Context.Interaction.RespondAsync(embed: first.SetPage(1, max, closeAt).Build(), components: components,
			ephemeral: true);

		var currentPage = 1;
		var current = first;
		while (DateTimeOffset.UtcNow < closeAt)
		{
			var interaction = await InteractionUtility.WaitForInteractionAsync(m_client, timeout, interaction =>
			{
				var componentInteraction = interaction as IComponentInteraction;
				return interaction.Type == InteractionType.MessageComponent && componentInteraction != null &&
				       (ids.Contains(componentInteraction.Data.CustomId) ||
				        componentInteraction.Data.CustomId.StartsWith("buy_"));
			});
			var componentInteraction = (interaction as IComponentInteraction)!;
			closeAt = DateTimeOffset.UtcNow.Add(timeout);

			await componentInteraction.DeferAsync(true);

			if (componentInteraction.Data.CustomId == ids[1]) break;

			if (componentInteraction.Data.CustomId == ids[0])
			{
				current = current.MoveLeft(ref currentPage, ref max, ref embeds);
				await interaction.UpdateShop(
					current, currentPage, max,
					closeAt,
					ids, profile, shop);
			}
			else if (componentInteraction.Data.CustomId == ids[2])
			{
				current = current.MoveRight(ref currentPage, ref max, ref embeds);
				await interaction.UpdateShop(
					current, currentPage, max,
					closeAt,
					ids, profile, shop);
			}
			else if (componentInteraction.Data.CustomId.StartsWith("buy_"))
			{
				var index = byte.Parse(componentInteraction.Data.CustomId.Split("_")[^1]);
				var item = await m_dataBaseProvider.GetItem(index);

				if (item == null)
				{
					await Context.Interaction.FollowupAsync("Item not found", ephemeral: true);
					return;
				}

				profile.Hearts -= shop.GetCost(item);
				profile.Inventory.Add($"{shop.Name}_{item.Name}_{item.Index}");
				await Context.Guild.GetUser(Context.User.Id).AddRoleAsync(item.Item);

				await m_dataBaseProvider.SetProfiles(profile);
				await Context.Interaction.FollowupAsync($"{item.Name} bought", ephemeral: true);

				await interaction.UpdateShop(
					current, currentPage, max,
					closeAt,
					ids, profile, shop);
			}
		}

		await Context.Interaction.CloseShop(shop, currentPage, current.Fields.Count, ids);
	}
}