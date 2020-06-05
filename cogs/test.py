import discord as d
from discord.ext import commands
import datetime


class Test(commands.Cog):
    def __init__(self, bot):
        self.bot = bot

    @commands.command(usage="test")
    @commands.is_owner()
    async def test(self, ctx):
        o = ctx.guild.get_member(348444859360608256)
        em = d.Embed(
                title=f"{ctx.author}\n**Забустил сервер**",
                description=f"{ctx.guild.get_role(709738102394191984).mention}\nОгромное спасибо, что помогаете серверу!!")
        em.set_thumbnail(url=ctx.author.avatar_url_as(static_format='png', size=512))
        em.set_image(url="https://images.app.goo.gl/YCaWRNtd8qHiziMs8")
        em.timestamp = datetime.datetime.utcnow()
        em.set_footer(text=f"{o.name}#{o.discriminator}\nБольшое спасибо передаёт администрация FT", icon_url=o.avatar_url_as(size=4096, static_format="png"))
        await ctx.send(embed = em)
            

    @commands.command(usage="react")
    @commands.is_owner()
    async def react(self, ctx, msg: int = None):
        if msg:
            msg = await ctx.guild.get_channel(702122569851076649).fetch_message(msg)

            for i in ["<:e_ftyes:701774227610796132>", "<:e_ftno:701774245746704436>"]:
                await msg.add_reaction(i)


def setup(bot):
    bot.add_cog(Test(bot))
