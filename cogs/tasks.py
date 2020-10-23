import threading

from discord.ext import commands as cmd
from discord.ext.tasks import loop


class Tasks(cmd.Cog):
    def __init__(self, bot):
        self.bot = bot

        for i in range(1):
            threading.Thread(target=self.time_update.start).start()

    @loop(seconds=15)
    async def time_update(self):
        channel = self.bot.get_channel(684011140908449843)
        name = self.bot.utils.tm("╽🎉ивенты")
        await channel.edit(reason="event time", name=name)


def setup(bot):
    bot.add_cog(Tasks(bot))
