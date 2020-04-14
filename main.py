import discord as d
from discord.ext import commands as c

b = c.Bot(command_prefix=None)

@b.event
async def on_member_join(m):
  e = d.Embed(
    title="",
    description=""
  )
  await b.get_channel().send(embed=e)

b.run()