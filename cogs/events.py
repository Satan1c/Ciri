import discord as d
import asyncio as ai

from discord.ext import commands



class Events(commands.Cog):
  def __init__(self, bot):
    self.bot = bot
    self.etime = bot.etime
  
  @commands.Cog.listener()
  async def on_ready(self):
    print("ready")
    await self.etime()

  @commands.Cog.listener()
  async def on_member_join(self, m):
    g = self.bot.get_guild(542005378049638400)
    c = self.bot.get_channel(639709192042709002)
    fc = self.bot.get_channel(684010692571037706)
    sc = self.bot.get_channel(542005378049638403)
    o = self.bot.get_user(348444859360608256)
    r = g.get_role(542012055775870976)

    e = d.Embed(title="\t🌞  🇯🇵  ヤーホー !  ( Yaho ! )  🇯🇵  🌞",description=f"\
      Приветствуем тебя, {m.mention}!\n\n\
      Только что Ты стал частью уютного сервера {g.name}!\n\
      👉 Обязательно загляни в канал #deleted-channel.n\n\
      Там ты найдешь всю важную инфу для комфортного времяпровождения на сервере (⌒▽⌒)❤️\n\n\
      💬 Если хочешь начать общение, просто напиши \"Привет всем\" в канал #deleted-channel!\n\n\
      Ты у нас уже - {len(m.guild.members)}-й гость.")
    
    e.set_thumbnail(url=m.avatar_url_as(size= 4096, format= None, static_format= "png"))
    e.set_footer(text=f"{o.name}#{o.discriminator}", icon_url=o.avatar_url_as(size= 4096, format= None, static_format= "png"))

    await m.add_roles(r, reason="new user")
    await c.send(embed=e)
    
  @commands.Cog.listener()
  async def on_member_remove(self, m):
    g = self.bot.get_guild(542005378049638400)
    r = g.get_role(542012055775870976)

    e = d.Embed(title="\t🌛  🇯🇵  さようなら!  ( Sayounara ! )   :flag_jp:   🌛",description=f"\
      Береги себя, {m.mention}!\n\n\
      Бyдем надеяться, что наш дрyг снова зяглянет к нам! :sparkles:.")
    
    e.set_thumbnail(url=m.avatar_url_as(size= 4096, format= None, static_format= "png"))
    e.set_footer(text=f"{o.name}#{o.discriminator}", icon_url=o.avatar_url_as(size= 4096, format= None, static_format= "png"))

    await m.add_roles(r, reason="new user")
    await c.send(embed=e)
    
  @commands.Cog.listener()
  async def on_message(self, msg):
    channel = self.bot.get_channel(702122569851076649).id
    if msg.channel.id == channel:
      await msg.add_reaction("<:e_ftyes:701774227610796132>")
      await msg.add_reaction("<:e_ftno:701774245746704436>")
  
def setup(bot):
  bot.add_cog(Events(bot))