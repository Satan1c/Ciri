import datetime

import discord
from discord.ext import commands as cmd

from bot import models
from bot.bot import Ciri


class Events(cmd.Cog):
    def __init__(self, bot: Ciri):
        self.bot = bot
        self.utils = bot.utils
        self.owner = self.bot.get_user(348444859360608256)

    @cmd.Cog.listener("on_voice_state_update")
    async def voice_update(self, member: discord.Member, before: discord.VoiceState, after: discord.VoiceState):
        if member.bot \
                or after.channel and isinstance(after.channel, discord.StageChannel) \
                or before.channel and isinstance(before.channel, discord.StageChannel):
            return

        cfg: dict = await self.bot.db.get_one_config({"_id": self.bot.db_enumeration.get("private_rooms")}, False)
        prf: models.User = models.User(
            (await self.bot.db.get_one_profile({"_id": member.id}, False, models.User.get_data(member)[0])))

        if after.channel and after.channel.id in list(cfg["channels"]):
            channel = await member.guild.get_channel(after.channel.category_id) \
                .create_voice_channel(f"{member.display_name}",
                                      user_limit=5,
                                      overwrites={
                                          member.guild.me: discord.PermissionOverwrite(manage_channels=True),
                                          member: discord.PermissionOverwrite(manage_channels=True),
                                      })

            await member.move_to(channel)

            prf.join_voice = datetime.datetime.utcnow()
            await self.bot.db.update_one_profile({"_id": member.id}, dict(prf.__dict__()))

        if before.channel and before.channel.id not in list(cfg['channels']) and before.channel.category_id in list(cfg['categories']) and not (before.channel.members and [i for i in before.channel.members if not i.bot]):
            prf.join_voice = None
            await self.bot.db.update_one_profile({"_id": member.id}, dict(prf.__dict__()))
            await before.channel.delete()

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
                title=f"{msg.author}\n**Забустил сервер**",
                description=f"{msg.guild.get_role(709738102394191984).mention}\nОгромное спасибо, что помогаете серверу!!")
            em.set_thumbnail(url=msg.author.avatar_url_as(static_format='png', size=512))
            em.set_image(url="https://thumbs.gfycat.com/ClumsyExcellentLeveret-size_restricted.gif")
            em.set_footer(text=f"{str(self.owner)}",
                          icon_url=self.owner.avatar_url_as(size=4096, static_format="png"))

            await msg.guild.get_channel(684011135287951392).send(embed=em)


def setup(bot):
    bot.add_cog(Events(bot))
