import discord as d
import asyncio as ai
from discord.ext import commands as c
from discord.ext.tasks import loop as l
from time import localtime

class Ciri(c.Bot):
  def __init__(self):
    super().__init__(command_prefix="adshgrsfhfhgk,jlij;lj'nhplftujxfvjf", case_insensitive=True)
    
    self.init()
    
  def init(self):
    self.prefix = "adshgrsfhfhgk,jlij;lj'nhplftujxfvjf"
    self.token = "Njk5NjczMzUzMDE1MDAxMTE4.XpXzuA.pGlWBQnL839E8uEoV_vV5nnYfLA"
    
  def tm(self, string: str = ""):
    hour = str(localtime().tm_hour + 3)
    minu = str(localtime().tm_min)
    tm = string + "〘" + hour + "∵" + minu + "〙"
    return str(tm)

  async def event_time(self):
    while 1:
      fc = self.get_channel(684011140908449843)
      n = self.tm("⸨🎉︙ивенты ")

      await fc.edit(reason="event time", name=n)
      
      await ai.sleep(10)
  
  async def on_ready(self):
    print("ready")
    await self.event_time()

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
    e.set_thumbnail(url=m.avatar_url)
    e.set_footer(text=f"{o.name}#{o.discriminator}", icon_url=o.avatar_url)

    await m.add_roles(r, reason="new user")
    await c.send(embed=e)

  
    

  def startup(self):
    super().run(self.token)

    
bot = Ciri()