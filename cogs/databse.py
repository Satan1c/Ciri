import json

from bot.bot import Ciri
from discord.ext import commands as cmd


class DataBase(cmd.Cog):
    def __init__(self, bot: Ciri):
        self.bot = bot

    @cmd.command(name="Config edit", alises=['config', "cfg"])
    @cmd.is_owner()
    async def config_command(self, ctx: cmd.Context, action: str, address: str, *, data: str):
        data = json.loads(data)
        content = self.bot.db_enumeration.get(data['content'])
        data['content'] = None

        if action == "create":
            db = self.bot.db.get_database("config").get_collection(address)
            _ = await db.find_one({"_id": content})

            if _:
                return await db.update_one({"_id": content}, {"$set": data})
            db.insert_one({"_id": content, "data": data})

        elif action == "edit":
            db = self.bot.db.get_database("config").get_collection(address)
            _ = await db.find_one({"_id": content})

            if _:
                return await db.update_one({"_id": content}, {"$set": data})
            db.insert_one({"_id": content, "data": data})

        elif action == "delete":
            db = self.bot.db.get_database("config").get_collection(address)
            _ = await db.find_one({"_id": content})

            if _:
                return await db.delete_one({"_id": content}, {"$set": data})
            raise cmd.BadArgument("No config found")


def setup(bot):
    bot.add_cog(DataBase(bot))
