from discord.ext import commands

class Test(commands.Cog):
  def __init__(self, bot):
    self.bot = bot
  @commands.command(usage="test")
  async def test(self, ctx):
    return await ctx.message.channel.send("done")
    
def setup(bot):
  bot.add_cog(Test(bot))