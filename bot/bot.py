import os

import discord
from config import token
from discord.ext import commands as cmd
from .utils import Utils


class Ciri(cmd.Bot):
    def __init__(self):
        super().__init__(command_prefix="+", case_insensitive=True, intents=discord.Intents.all())

    def load(self):
        self.utils = Utils(self)
        self.remove_command('help')

        for file in os.listdir('./cogs'):
            if file[-3:] == '.py':
                try:
                    self.load_extension(f'cogs.{file[0:-3]}')
                    print(f'[+] cogs.{file[0:-3]}')
                except BaseException as err:
                    print(f'[!] cogs.{file[0:-3]} error: `{err}`')
        print('-' * 30)

    async def on_command_error(self, ctx: cmd.Context, err):
        if not ctx.command:
            return print(err)
        embed = discord.Embed(title=ctx.command.name + " error", description=str(err))
        await ctx.send(embed=embed)

    async def on_ready(self):
        self.load()
        await self.change_presence(status=discord.Status.dnd, activity=discord.Activity(name="prefix +", type=discord.ActivityType.listening))
        print(self.user.name, "is ready")

    def run(self):
        super().run(token)


bot = Ciri()
