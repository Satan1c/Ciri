import discord as d
import asyncio as ai

from discord.ext import commands



class Events(commands.Cog):
  def __init__(self, bot):
    self.bot = bot
  
  @commands.Cog.listener()
  async def on_ready(self):
    
    print("ready")
    await self.etime()

  @commands.Cog.listener()
  async def on_member_join(self, m):
    c = self.get_channel(639709192042709002)
    fc = self.get_channel(684010692571037706)
    sc = self.get_channel(542005378049638403)
    o = self.get_user(348444859360608256)
    r = self.get_guild(542005378049638400).get_role(542012055775870976)

    e = d.Embed(title="\t**Добро пожаловать**",description=f"\
      Привет, {m.mention}! Рад видеть тебя в нашем уютном уголке - {m.guild.name}.\n\n\
      👉 **Обязательно загляни в канал** {fc.mention}!\n\
      Там ты найдешь всю важную инфу для комфортного времяпровождения на сервере (⌒▽⌒)♡\n\n\
      💜 **Если хочешь начать общение, просто напиши \"Привет всем\" в канал** {sc.mention}!\n\n\
      Ты у нас уже - {len(m.guild.members)}-й гость.")
    
    e.set_thumbnail(url=m.avatar_url_as(size= 4096, format= None, static_format= "png"))
    e.set_footer(text=f"{o.name}#{o.discriminator}", icon_url=o.avatar_url_as(size= 4096, format= None, static_format= "png"))

    await m.add_roles(r, reason="new user")
    await c.send(embed=e)
    
  @commands.Cog.listener()
  async def on_message(self, msg):
    channel = self.get_channel(702122569851076649).id
    if msg.channel.id == channel:
      await msg.add_reaction("<:e_ftyes:701774227610796132>")
      await msg.add_reaction("<:e_ftno:701774245746704436>")
  
def setup(bot):
  bot.add_cog(Events(bot))