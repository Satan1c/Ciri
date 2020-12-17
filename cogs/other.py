import discord
from bot.bot import Ciri
from discord.ext import commands as cmd


class Other(cmd.Cog):
    def __init__(self, bot: Ciri):
        self.bot = bot

    @cmd.command(name="Poll", aliases=["предложение"])
    @cmd.guild_only()
    async def poll(self, ctx: cmd.Context, *, text: str = None):
        if not text:
            raise cmd.BadArgument("Give some text")

        embed = discord.Embed(description=text, colour=discord.Colour.from_rgb(250, 240, 120))
        embed.set_author(name=str(ctx.author), icon_url=ctx.author.avatar_url)

        msg = await ctx.guild.get_channel(702122569851076649).send(embed=embed)

        for i in ["⬆️", "⬇️"]:
            await msg.add_reaction(i)

    @cmd.command(Name="Test", hidden=True)
    @cmd.is_owner()
    async def test(self, ctx: cmd.Context):
        res = await self.bot.profiles.find_one({"_id": 548854261144879124})
        print(res)


def setup(bot):
    bot.add_cog(Other(bot))
