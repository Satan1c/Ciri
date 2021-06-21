import json
from typing import List

import discord
from discord.ext import commands as cmd

from bot.bot import Ciri


class DataBase(cmd.Cog):
    def __init__(self, bot: Ciri):
        self.bot = bot

    @cmd.command(name="Private roms", aliases=['pr'])
    @cmd.has_guild_permissions(manage_messages=True)
    @cmd.guild_only()
    async def private_rooms(self, ctx: cmd.Context, action: str , *, channels_raw):
        channels_raw = [ctx.guild.get_channel(int(i)) for i in channels_raw.split(" ")]
        channels: List[int] = [i.id for i in channels_raw if i]
        categories: List[int] = [i.category_id for i in channels_raw if i]
        cfg: dict = await self.bot.db.get_one_config({"_id": self.bot.db_enumeration.get("private_rooms")})
        chs: List[int] = cfg["channels"]
        cts: List[int] = cfg["categories"]

        if action == "add":
            if channels not in chs:
                chs.extend(channels)
                chs = list(set(chs))

            if categories not in chs:
                cts.extend(categories)
                cts = list(set(cts))

        elif action == "remove":
            chs = [i for i in chs if i not in channels]
            cts = [i for i in cts if i not in categories]

        cfg["channels"] = chs
        cfg["categories"] = cts

        await self.bot.db.update_one_config({"_id": cfg["_id"]}, cfg)

    @cmd.command(name="Config edit", aliases=['config', "cfg"])
    @cmd.has_guild_permissions(manage_guild=True)
    @cmd.guild_only()
    async def config_command(self, ctx: cmd.Context, action: str, *, data: str):
        data: dict = json.loads(data)
        content: str = self.bot.db_enumeration.get(data['content'])
        content = content if content else int(data['content'])
        data['content'] = None

        if action in ["create", "edit"]:
            await self.bot.db.update_one_config({"_id": content}, {"$set": data})

        elif action == "delete":
            await self.bot.db.delete_one_config({"_id": content})

        else:
            raise cmd.BadArgument("Invalid action")


def setup(bot):
    bot.add_cog(DataBase(bot))
