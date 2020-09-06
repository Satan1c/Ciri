import discord as d

from discord.ext import commands


class Events(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self.etime = bot.etime
        self.bump = bot.bump

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

        e = d.Embed(colour=d.Colour.from_rgb(54, 57, 63), title="\t🌞  🇯🇵  ヤーホー !  ( Yaho ! )  🇯🇵  🌞",
                    description=f"\
            Приветствуем тебя, {m.mention}!\n\n\
            Только что **Ты** стал частью уютного сервера\n{g.name}!\n\
            👉 Обязательно загляни в канал {fc.mention}.\n\n\
            Там ты найдешь всю важную инфу для комфортного времяпровождения на сервере (⌒▽⌒)❤️\n\n\
            💬 Если хочешь начать общение,\nпросто напиши \"Привет всем\" в канал {sc.mention}!\n\n\
            Ты у нас уже - {len(m.guild.members)}-й гость.")

        e.set_thumbnail(url=m.avatar_url_as(size=4096, format=None, static_format="png"))
        e.set_footer(text=f"{o.name}#{o.discriminator}",
                     icon_url=o.avatar_url_as(size=4096, format=None, static_format="png"))

        await m.add_roles(r, reason="new user")
        await c.send(embed=e)

    @commands.Cog.listener()
    async def on_member_remove(self, m):
        g = self.bot.get_guild(542005378049638400)
        r = g.get_role(542012055775870976)
        o = self.bot.get_user(348444859360608256)
        c = self.bot.get_channel(639709192042709002)

        e = d.Embed(colour=d.Colour.from_rgb(54, 57, 63), title="\t🌛  🇯🇵  さようなら!  ( Sayounara ! )  🇯🇵  🌛", description=f"\
            Береги себя, {m.mention}!\n\n\
            Бyдем надеяться, что наш дрyг снова зяглянет к нам! ✨.")

        e.set_thumbnail(url=m.avatar_url_as(size=4096, format=None, static_format="png"))
        e.set_footer(text=f"{o.name}#{o.discriminator}", icon_url=o.avatar_url_as(size=4096, static_format="png"))

        await c.send(embed=e)

    @commands.Cog.listener()
    async def on_message(self, msg):
        o = self.bot.get_user(348444859360608256)

        if msg.author.id == 315926021457051650 or msg.author.id == 464272403766444044:
            em = msg.embeds[0]
            self.bot.get_user(348444859360608256).send(f"{dict(em)}")

        if msg.type == d.MessageType.premium_guild_subscription:
            em = d.Embed(
                title=f"{msg.author}\n**Забустил сервер**\n{msg.guild.get_role(709738102394191984).mebtion}",
                description="Огромное спасибо, что помогаете серверу!!")
            em.set_thumbnail(url=msg.author.avatar_url_as(static_format='png', size=512))
            em.set_image(url="https://thumbs.gfycat.com/ClumsyExcellentLeveret-size_restricted.gif")
            em.set_footer(text=f"{o.name}#{o.discriminator}", icon_url=o.avatar_url_as(size=4096, static_format="png"\
                                                                                      "Большое спасибо передаёт администрация FT"))

            await msg.guild.get_channel(684011135287951392).send(embed=em)


def setup(bot):
    bot.add_cog(Events(bot))
