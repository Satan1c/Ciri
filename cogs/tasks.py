from discord.ext import commands as cmd
from discord.ext.tasks import loop

from bot.bot import Ciri


class Tasks(cmd.Cog):
    def __init__(self, bot: Ciri):
        self.bot = bot
        self.time_update.start()
        # self.db_update.start()

    @loop(minutes=1)
    async def time_update(self):
        channel = self.bot.get_channel(770669968329146378)
        name = self.bot.utils.tm("🕐 Время: ")
        await channel.edit(reason="time", name=name)

    # @loop(hours=4)
    # async def db_update(self):
    #     arr = [int(i['_id']) async for i in self.bot.profiles.find()]
    #     add = [i for i in self.bot.get_guild(542005378049638400).members if i.id not in arr]
    #
    #     if len(add):
    #         await self.bot.profiles.insert_many(self.bot.models.User.get_data(add))
    #         print(len(add), "users created")


def setup(bot):
    bot.add_cog(Tasks(bot))
