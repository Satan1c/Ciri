import discord as d
from discord.ext import commands as c

b = c.Bot(command_prefix=None)

@b.event
async def on_ready():
  print("ready")

@b.event
async def on_member_join(m):
  c = b.get_channel(639709192042709002)
  fc = b.get_channel()
  sc = b.get_channel()
  o = b.get_user(348444859360608256)
  
  e = d.Embed(title="\t**Добро пожаловать**",description=f"\
    Привет, {m.name}! Рад видеть тебя в нашем уютном уголке - <названиесервера>.\n\n \
    👉 **Обязательно загляни в канал** {fc.mention}!\n \
    Там ты найдешь всю важную инфу для комфортного времяпровождения на сервере (⌒▽⌒)♡\n\n \
    💜 **Если хочешь начать общение, просто напиши \"Привет всем\" в канал** {sc.mention}!\n\n \
    Ты у нас уже - {len(m.guild.members)} гость.")
  e.set_thumbnail(url=m.avatar_url)
  e.set_footer(text=o.name, icon_url=o.avatar_url)
  
  await m.add_roles(b.get_guild(542005378049638400).get_role(542012055775870976), reason="new user")
  await c.send(embed=e)

b.run("Njk5NjczMzUzMDE1MDAxMTE4.XpXzuA.pGlWBQnL839E8uEoV_vV5nnYfLA")