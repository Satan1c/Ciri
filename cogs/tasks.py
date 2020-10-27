import threading

from discord.ext import commands as cmd
from discord.ext.tasks import loop
from bot.bot import Ciri


class Tasks(cmd.Cog):
    def __init__(self, bot: Ciri):
        self.bot = bot

        for i in range(1):
            threading.Thread(target=self.time_update.start).start()

    @loop(seconds=15)
    async def time_update(self):
        channel = self.bot.get_channel(770669968329146378)
        name = self.bot.utils.tm("Время: ")
        await channel.edit(reason="time", name=name)


def setup(bot):
    bot.add_cog(Tasks(bot))
