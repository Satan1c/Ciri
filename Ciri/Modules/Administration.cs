using Ciri.Models;
using DataBase;
using DataBase.Models;
using Discord;
using Discord.Interactions;

namespace Ciri.Modules;

[Group("administration", "administration commands")]
[DefaultMemberPermissions(GuildPermission.Administrator)]
[EnabledInDm(false)]
public class Administration : InteractionModuleBase<SocketInteractionContext>
{
	public static DataBaseProvider DataBaseProvider = null!;
	
	public Administration(DataBaseProvider dataBaseProvider)
	{
		Administration.DataBaseProvider = dataBaseProvider;
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
			
			var profile = newProfile.EditProfile(await DataBaseProvider.GetProfiles(user.Id));
			
			await DataBaseProvider.SetProfiles(profile);
			await Context.Interaction.RespondAsync($"<@{user.Id}> profile edited", ephemeral: true);
		}
	}
	
	[Group("shop", "shop commands")]
	public class Shop : InteractionModuleBase<SocketInteractionContext>
	{
		[SlashCommand("add", "add shop item command")]
		public async Task AddShopItem(
			[ComplexParameter]
			ItemArg<IRole> item)
		{
			var shop = await DataBaseProvider.GetShop<ulong>();
			if (shop == null)
			{
				await DataBaseProvider.SetShop(new Shop<ulong>
				{
					Discount = 0,
					Items = new List<ShopItem<ulong>>(),
					Name = "roles"
				});
			}
			
			await DataBaseProvider.SetItem(item.CreateShopItem(item.Item.Id));
			await Context.Interaction.RespondAsync($"{item.Name} added", ephemeral: true);
		}
		
		[SlashCommand("edit", "edit shop item command")]
		public async Task EditShopItem(
			[ComplexParameter]
			NullableItemArg<IRole> item, 
			byte? newIndex = null)
		{
			var find = await DataBaseProvider.GetItem<ulong>(index: item.Index);
			if (find == null)
			{
				await Context.Interaction.RespondAsync("Item not found", ephemeral: true);
				return;
			}

			find = item.CreateShopItem(item.Item?.Id ?? find.Item, find);

			await DataBaseProvider.SetItem(find);
			await Context.Interaction.RespondAsync($"{item.Name} edited", ephemeral: true);
		}
		
		[SlashCommand("remove", "remove shop item command")]
		public async Task RemoveShopItem(byte index, ShopType type)
		{
			var item = await DataBaseProvider.GetItem<object>(type.ShopTypeToString(), index);
			if (item == null)
			{
				await Context.Interaction.RespondAsync("Item not found", ephemeral: true);
				return;
			}
			
			await DataBaseProvider.RemoveItem<object>(index);
			await Context.Interaction.RespondAsync($"{item.Name} removed", ephemeral: true);
		}
	}
}

public enum ShopType
{
	[ChoiceDisplay("roles")]
	Roles,
}

public static class Extensions
{
	public static string ShopTypeToString(this ShopType type)
	{
		return type switch
		{
			ShopType.Roles => "roles",
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
	}
}