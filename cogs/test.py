import discord as d
from discord.ext import commands as cmd
import datetime


class Test(cmd.Cog):
    def __init__(self, bot):
        self.bot = bot

    @cmd.command()
    @cmd.is_owner()
    async def test(self, ctx):
        await ctx.send(ctx.message.clean_content)
            

    @cmd.command()
    @cmd.is_owner()
    async def react(self, ctx, msg: int = None):
        if msg:
            msg = await ctx.guild.get_channel(702122569851076649).fetch_message(msg)

            for i in ["<:e_ftyes:701774227610796132>", "<:e_ftno:701774245746704436>"]:
                await msg.add_reaction(i)
    
    @cmd.command()
    @cmd.guild_only()
    async def created_at(self, ctx):
        date = ctx.guild.created_at
        res = "{0.day}/{0.month}/{0.year} {0.hour}:{0.minute}:{0.second}".format(date)
        await ctx.send(res)
        
    @cmd.command(aliases=['предложение'])
    @cmd.guild_only()
    async def poll(self, ctx: cmd.Context):
        text = " ".join(ctx.message.clean_content.split(" ")[1:])
        if not text:
          return ctx.channel.send("Give some `text`")
      
        msg = await ctx.guild.get_channel(702122569851076649).send(
          embed=d.Embed(description=text, colour=d.Colour.from_rgb(250, 240, 120))
          .set_author(name=str(ctx.author), icon_url=ctx.author.avatar_url)
        )
      
        await msg.add_reaction("⬆️")
        await msg.add_reaction("⬇️")
      
        await ctx.message.delete()


def setup(bot):
    bot.add_cog(Test(bot))
