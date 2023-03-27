namespace Ciri.Modules.Configs;

public static class ImagesConfig
{
	public static ImagesConfigSingleton Instance { get; } = new ();
	
	public const string MafiaImage = "https://images-ext-2.discordapp.net/external/HHcqJf83OAtgxlAMK5DWnoJE0HyJcn_cBn3JgevCM8M/https/images-ext-1.discordapp.net/external/8CfeMCv8vAqqbbbQ1Eqaf0o7seLGp4Vxrqyj3ZYNURI/%253Fwidth%253D1216%2526height%253D684/https/media.discordapp.net/attachments/767725811905593346/779325292954124298/-1.png?width=1214&height=683";
	public const string AliasImage = "https://images-ext-2.discordapp.net/external/o2c-iffYr-FdMEt4rPs6dsEFG2DCCqzwPN-82FQqArk/%3Fwidth%3D1186%26height%3D683/https/images-ext-2.discordapp.net/external/0G0tUp_W8cbm0rGqEFiBjW4pSDqAD8B36Otqlr1TzbY/%253Fwidth%253D1188%2526height%253D684/https/media.discordapp.net/attachments/691511839942508554/710512695241211995/238_20200514192314.png";
	public const string WhoIImage = "https://media.discordapp.net/attachments/691511839942508554/718420458923491388/308_20200605150614.png?width=1186&height=683";
	public const string CrocodileImage = "https://images-ext-1.discordapp.net/external/VEc5oQLU_znJ2ygkpt5eUkt0FrKr_ZLGXREJGi3Iw94/https/i.ibb.co/4gGQNcr/crocodile.png?width=1368&height=683";
	public const string JackboxImage = "https://media.discordapp.net/attachments/767725811905593346/779741553634574366/JackBox4-revive.png?width=1094&height=683";
	public const string TabletopImage = "https://media.discordapp.net/attachments/699990941439754371/1082293035095834624/nastolkievent.png?width=1368&height=683";
	public const string MomentsImage = "https://media.discordapp.net/attachments/669887782117703701/781572552580530216/5ee6b1c039ce0f6b.png?width=1214&height=683";

}

public class ImagesConfigSingleton
{
	public string MafiaImage => ImagesConfig.MafiaImage;
	public string AliasImage => ImagesConfig.AliasImage;
	public string WhoIImage => ImagesConfig.WhoIImage;
	public string CrocodileImage => ImagesConfig.CrocodileImage;
	public string JackboxImage => ImagesConfig.JackboxImage;
	public string TabletopImage => ImagesConfig.TabletopImage;
	public string MomentsImage => ImagesConfig.MomentsImage;
}