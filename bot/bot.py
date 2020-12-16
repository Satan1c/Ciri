import os

from motor.motor_asyncio import AsyncIOMotorClient

import discord
from config import token, mongo
from discord.ext import commands as cmd
from . import models
from .utils import Utils


class Ciri(cmd.Bot):
    def __init__(self):
        super().__init__(command_prefix=self.get_prefix, case_insensitive=True, intents=discord.Intents.all())
        self.client = AsyncIOMotorClient(mongo)

    def load(self):
        self.models = models
        self.utils = Utils(self)
        self.db = self.client
        self.config = self.client.get_database("config").get_collection("main")
        self.profiles = self.client.get_database("users").get_collection("profiles")
        self.server = self.client.get_database("server")
        self.db_enumeration = {
            "main": 0,
            "profile": 1,
            "rep": 2,
            "add_rep": 3
        }
        self.remove_command('help')

        for file in os.listdir('./cogs'):
            if file[-3:] == '.py':
                try:
                    self.load_extension(f'cogs.{file[0:-3]}')
                    print(f'[+] cogs.{file[0:-3]}')
                except BaseException as err:
                    print(f'[!] cogs.{file[0:-3]} error: `{err}`')
        print('-' * 30)

    async def on_command(self, ctx: cmd.Context):
        if not ctx.author.bot and ctx.message.guild:
            try:
                await ctx.message.delete()
            except:
                pass

    async def get_prefix(self, message: discord.Message):
        if not message.author.bot:
            return "t+"

    async def on_command_error(self, ctx: cmd.Context, err):
        if not ctx.command:
            return print(err)
        embed = discord.Embed(title=ctx.command.name + " error", description=str(err))
        embed.set_footer(text=ctx.command.usage)
        await ctx.send(embed=embed)

    async def on_ready(self):
        self.load()
        await self.change_presence(status=discord.Status.dnd,
                                   activity=discord.Activity(name="prefix +", type=discord.ActivityType.listening))
        print(self.user.name, "is ready")

    def run(self):
        super().run(token)


bot = Ciri()
