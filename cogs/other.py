import discord
from discord.ext import commands as cmd


class Other(cmd.Cog):
    def __init__(self, bot):
        self.bot = bot

    @cmd.command(name="Poll", aliases=["предложение"])
    @cmd.guild_only()
    @cmd.has_guild_permissions(manage_messages=True)
    async def poll(self, ctx: cmd.Context, *, text: str = None):
        await ctx.message.delete()
        if not text:
            raise cmd.BadArgument("Give some text")

        embed = discord.Embed(description=text, colour=discord.Colour.from_rgb(250, 240, 120))
        embed.set_author(name=str(ctx.author), icon_url=ctx.author.avatar_url)

        msg = await ctx.guild.get_channel(702122569851076649).send(embed=embed)

        for i in ["⬆️", "⬇️"]:
            await msg.add_reaction(i)

    @cmd.command(Name="Test")
    @cmd.is_owner()
    async def test(self, ctx: cmd.Context, mid: int):
        await ctx.message.delete()
        message = await ctx.channel.fetch_message(mid)
        print(message.embeds[0].to_dict())


def setup(bot):
    bot.add_cog(Other(bot))
