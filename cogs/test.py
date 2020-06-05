import discord as d
from discord.ext import commands


class Test(commands.Cog):
    def __init__(self, bot):
        self.bot = bot

    @commands.command(usage="test")
    @commands.is_owner()
    async def test(self, ctx, eid: int):
        emoji = await self.bot.get_guild(542005378049638400).fetch_emoji(eid)
        await ctx.message.channel.send(emoji.url)

    @commands.command(usage="react")
    @commands.is_owner()
    async def react(self, ctx, msg: int = None):
        if msg:
            msg = await ctx.guild.get_channel(702122569851076649).fetch_message(msg)

            for i in ["<:e_ftyes:701774227610796132>", "<:e_ftno:701774245746704436>"]:
                await msg.add_reaction(i)


def setup(bot):
    bot.add_cog(Test(bot))
