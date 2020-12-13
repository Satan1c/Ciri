from discord.ext import commands as cmd


class Economics(cmd.Cog):
    def __init__(self, bot):
        self.bot = bot


def setup(bot):
    bot.add_cog(Economics(bot))
