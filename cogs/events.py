import discord
from bot.bot import Ciri
from discord.ext import commands as cmd


class Events(cmd.Cog):
    def __init__(self, bot: Ciri):
        self.bot = bot
        self.utils = bot.utils
        self.owner = self.bot.get_user(348444859360608256)

    @cmd.Cog.listener("on_member_join")
    async def member_join(self, member: discord.Member):
        channel = self.bot.get_channel(639709192042709002)
        first = self.bot.get_channel(684010692571037706)
        second = self.bot.get_channel(542005378049638403)
        role = member.guild.get_role(542012055775870976)

        embed = discord.Embed(colour=discord.Colour.from_rgb(54, 57, 63),
                              title="\t🌞  🇯🇵  ヤーホー !  ( Yaho ! )  🇯🇵  🌞",
                              description=f"\
                    Приветствуем тебя, {member.mention}!\n\n\
                    Только что **Ты** стал частью уютного сервера\n{member.guild.name}!\n\
                    👉 Обязательно загляни в канал {first.mention}.\n\n\
                    Там ты найдешь всю важную инфу для комфортного времяпровождения на сервере (⌒▽⌒)❤️\n\n\
                    💬 Если хочешь начать общение,\nпросто напиши \"Привет всем\" в канал {second.mention}!\n\n\
                    Ты у нас уже - {len(member.guild.members)}-й гость.")

        embed.set_thumbnail(url=member.avatar_url_as(size=4096, format=None, static_format="png"))
        embed.set_footer(text=f"{self.owner.name}#{self.owner.discriminator}",
                         icon_url=self.owner.avatar_url_as(size=4096, format=None, static_format="png"))

        await member.add_roles(role, reason="new user")
        await channel.send(embed=embed)

        prf = await self.bot.profiles.find_one({"_id": member.id})
        if not prf:
            await self.bot.profiles.insert_one(self.bot.models.User.get_data(member)[0])

    @cmd.Cog.listener("on_member_remove")
    async def member_remove(self, member):
        channel = self.bot.get_channel(639709192042709002)

        embed = discord.Embed(colour=discord.Colour.from_rgb(54, 57, 63),
                              title="\t🌛  🇯🇵  さようなら!  ( Sayounara ! )  🇯🇵  🌛",
                              description=f"\
                Береги себя, {member.mention}!\n\n\
                Бyдем надеяться, что наш дрyг снова зяглянет к нам! ✨.")

        embed.set_thumbnail(url=member.avatar_url_as(size=4096, format=None, static_format="png"))
        embed.set_footer(text=f"{str(self.owner)}", icon_url=self.owner.avatar_url_as(size=4096, static_format="png"))

        await channel.send(embed=embed)

    @cmd.Cog.listener("on_message")
    async def message(self, msg: discord.Message):
        if msg.type == discord.MessageType.premium_guild_subscription:
            em = discord.Embed(
                title=f"{msg.author}\n**Забустил сервер**\n{msg.guild.get_role(709738102394191984).mention}",
                description="Огромное спасибо, что помогаете серверу!!")
            em.set_thumbnail(url=msg.author.avatar_url_as(static_format='png', size=512))
            em.set_image(url="https://thumbs.gfycat.com/ClumsyExcellentLeveret-size_restricted.gif")
            em.set_footer(text=f"{str(self.owner)}",
                          icon_url=self.owner.avatar_url_as(size=4096, static_format="png"))

            await msg.guild.get_channel(684011135287951392).send(embed=em)

        if msg.author.id in [315926021457051650, 464272403766444044]:
            if len(msg.embeds) > 0:
                data = msg.embeds[0].to_dict()

                if "title" in data and data['title'] == "Сервер Up":
                    await self.utils.up_remind(msg.channel.id, "s.up")

                elif "description" in data and \
                        data['description'].startswith("[Top Discord Servers](https://discord-server.com/)"):
                    await self.utils.up_remind(msg.channel.id, "!bump")


def setup(bot):
    bot.add_cog(Events(bot))
