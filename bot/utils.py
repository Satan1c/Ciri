import asyncio
from time import localtime


class Utils:
    def __init__(self, bot):
        self.bot = bot

    def tm(self, string: str = ""):
        hour = str(localtime().tm_hour + 3)
        if int(hour) >= 24:
            hour = str(int(hour) - 24)
        minu = str(localtime().tm_min)
        tm = string + "〘" + (hour if len(hour) == 2 else "0" + hour) + "∵" + (
            minu if len(minu) == 2 else "0" + minu) + "〙"

        return str(tm)

    async def up_remind(self, channel: int, command: str):
        await asyncio.sleep(delay=4 * 60 * 60)

        role = self.bot.get_guild(542005378049638400).get_role(709306526094983209)
        await self.bot.get_channel(channel).send(f"{role.mention} it's time to {command}")
