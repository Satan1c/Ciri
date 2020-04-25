import discord as d
import asyncio as ai
import os

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
    self.etime = self.event_time
    
    for file in os.listdir('./cogs'):
      if file[-3:] == '.py':
        try:
          self.load_extension(f'cogs.{file[0:-3]}')
          print(f'[+] cogs.{file[0:-3]}')
        except BaseException as err:
          print(f'[!] cogs.{file[0:-3]} error: `{err}`')
    print('-' * 30)
    
  def tm(self, string: str = ""):
    hour = str(localtime().tm_hour + 3)
    if int(hour) >= 24:
      hour = str(int(hour)-24)
    minu = str(localtime().tm_min)
    tm = string + "〘" + (hour if len(hour) == 2 else "0" + hour) + "∵" + (minu if len(minu) == 2 else "0" + minu) + "〙"
    return str(tm)

  
  async def event_time(self):
    while 1:
      fc = self.get_channel(684011140908449843)
      n = self.tm("╽🎉ивенты")

      await fc.edit(reason="event time", name=n)
      
      await ai.sleep(15)
  
  def startup(self):
    super().run(self.token)

    
bot = Ciri()