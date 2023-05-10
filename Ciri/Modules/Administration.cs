using Ciri.Handlers;
using Ciri.Models;
using DataBase;
using DataBase.Models;
using Discord;
using Discord.Interactions;

namespace Ciri.Modules;

[Group("admin", "administration commands")]
[DefaultMemberPermissions(GuildPermission.Administrator)]
[EnabledInDm(false)]
public class Administration : InteractionModuleBase<SocketInteractionContext>
{
	private static DataBaseProvider s_dataBaseProvider = null!;

	public Administration(DataBaseProvider dataBaseProvider)
	{
		s_dataBaseProvider = dataBaseProvider;
	}

	[Group("profile", "profile commands")]
	public class Profile : InteractionModuleBase<SocketInteractionContext>
	{
		[SlashCommand("set", "set profile command")]
		public async Task SetProfile(IUser user, [ComplexParameter] ProfileArg newProfile)
		{
			if (newProfile.IsEmpty())
			{
				await Context.Interaction.RespondAsync("You must specify at least one parameter", ephemeral: true);
				return;
			}

			var profile = newProfile.EditProfile(await s_dataBaseProvider.GetProfiles(user.Id));

			await s_dataBaseProvider.SetProfiles(profile);
			await Context.Interaction.RespondAsync($"<@{user.Id}> profile edited", ephemeral: true);
		}
	}

	[Group("shop", "shop commands")]
	public class Shop : InteractionModuleBase<SocketInteractionContext>
	{
		[SlashCommand("add", "add shop item command")]
		public async Task AddShopItem(
			[ComplexParameter] ItemArg item)
		{
			var shop = await s_dataBaseProvider.GetShop();
			if (shop.AreSame(default))
				await s_dataBaseProvider.SetShop(new DataBase.Models.Shop
				{
					Discount = 0,
					Items = new List<ShopItem>(),
					Name = "roles"
				});

			await s_dataBaseProvider.SetItem(item.CreateShopItem(item.Item.Id));
			await Context.Interaction.RespondAsync($"{item.Name} added", ephemeral: true);
		}

		[SlashCommand("edit", "edit shop item command")]
		public async Task EditShopItem(
			[ComplexParameter] NullableItemArg item,
			byte? newIndex = null)
		{
			var find = await s_dataBaseProvider.GetItem(item.Index);
			if (find.AreSame(default))
			{
				await Context.Interaction.RespondAsync("Item not found", ephemeral: true);
				return;
			}

			find = item.CreateShopItem(item.Item?.Id ?? find.Item, find);

			await s_dataBaseProvider.SetItem(find, newIndex ?? item.Index);
			await Context.Interaction.RespondAsync($"{item.Name} edited", ephemeral: true);
		}

		[SlashCommand("remove", "remove shop item command")]
		public async Task RemoveShopItem([Autocomplete(typeof(ItemAutocomplete))] byte index)
		{
			var item = await s_dataBaseProvider.GetItem(index);
			if (item.AreSame(default))
			{
				await Context.Interaction.RespondAsync("Item not found", ephemeral: true);
				return;
			}

			await s_dataBaseProvider.RemoveItem(index);
			await Context.Interaction.RespondAsync($"{item.Name} removed", ephemeral: true);
		}
	}
}

/*public enum ShopType
{
	[ChoiceDisplay("roles")]
	Roles,
}*/
/*public static class UnsafeExtensions
{
	public static string ShopTypeToString(this ShopType type)
	{
		return type switch
		{
			ShopType.Roles => "roles",
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
	}
}*/