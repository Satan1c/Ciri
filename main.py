import discord as d
from discord.ext import commands as c

b = c.Bot(command_prefix=None)

@b.event
async def on_ready():
  print("ready")

@b.event
async def on_member_join(m):
  e = d.Embed(
    title="\t**Добро пожаловать**",
    description=f"Привет, {m.name}! Рад видеть тебя в нашем уютном уголке - <названиесервера>."
  ).set_thumbnail(url=m.avatar_url)
  
  await m.add_roles(b.get_guild(542005378049638400).get_role(542012055775870976), reason="new user")
  await b.get_channel(639709192042709002).send(embed=e)

b.run("Njk5NjczMzUzMDE1MDAxMTE4.XpXzuA.pGlWBQnL839E8uEoV_vV5nnYfLA")