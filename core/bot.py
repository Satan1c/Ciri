import asyncio as ai
import os
from multiprocessing import Process as p
from time import localtime

from discord.ext import commands as c

from .serv import app


class Ciri(c.Bot):
    def __init__(self):
        super().__init__(command_prefix="Ciri ", owner=348444859360608256)

        self.init()

    def init(self):
        self.token = "Njk5NjczMzUzMDE1MDAxMTE4.XxhUbg.yMFXMdf65sjGacDX7gSHgGoTy7c"
        self.etime = self.event_time

        for file in os.listdir('./cogs'):
            if file[-3:] == '.py':
                try:
                    self.load_extension(f'cogs.{file[0:-3]}')
                    print(f'[+] cogs.{file[0:-3]}')
                except BaseException as err:
                    print(f'[!] cogs.{file[0:-3]} error: `{err}`')
        print('-' * 30)

    def _tm(self, string: str = ""):
        hour = str(localtime().tm_hour + 3)
        if int(hour) >= 24:
            hour = str(int(hour) - 24)
        minu = str(localtime().tm_min)
        tm = string + "〘" + (hour if len(hour) == 2 else "0" + hour) + "∵" + (
            minu if len(minu) == 2 else "0" + minu) + "〙"

        return str(tm)

    async def event_time(self):
        while 1:
            fc = self.get_channel(684011140908449843)
            n = self._tm("╽🎉ивенты")
            await fc.edit(reason="event time", name=n)

            await ai.sleep(120)

    async def on_connect(self):
        p(target=app.run).start()

    async def bump(self, cmd):
        ai.sleep(14340)
        ch = self.get_channel(684011228531654658)
        msg = f"<@&709306526094983209> начинайте писать {cmd}"
        await ch.send(msg)

    async def on_command_error(self, ctx, err):
        print(err)

    def startup(self):
        super().run(self.token)


bot = Ciri()
