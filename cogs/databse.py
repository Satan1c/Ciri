import json

from bot.bot import Ciri
from discord.ext import commands as cmd


class DataBase(cmd.Cog):
    def __init__(self, bot: Ciri):
        self.bot = bot

    @cmd.command(name="Config edit", aliases=['config', "cfg"])
    @cmd.has_guild_permissions(manage_messages=True)
    @cmd.guild_only()
    async def config_command(self, ctx: cmd.Context, action: str, *, data: str):
        data = json.loads(data)
        content = self.bot.db_enumeration.get(data['content'])
        data['content'] = None

        db = await self.bot.config.find_one({"_id": content})

        if action in ["create", "edit"]:
            if db:
                return await db.update_one({"_id": content}, {"$set": data})
            self.bot.config.insert_one({"_id": content})

        elif action == "delete":
            if db:
                return await db.delete_one({"_id": content}, {"$set": data})
            raise cmd.BadArgument("No config found")

        else:
            raise cmd.BadArgument("Invalid action")


def setup(bot):
    bot.add_cog(DataBase(bot))
