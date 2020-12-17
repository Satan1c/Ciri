import discord
from bot.bot import Ciri
from discord.ext import commands as cmd


class Economics(cmd.Cog):
    def __init__(self, bot: Ciri):
        self.bot = bot
        self.profiles = bot.profiles

    @cmd.command(name="Balance", aliases=['bal', 'баланс', "бал"], usage="баланс")
    @cmd.guild_only()
    async def balance(self, ctx: cmd.Context):
        config = await self.bot.config.find_one({"_id": self.bot.db_enumeration.get("balance")})
        config = config['data']
        profile = await self.profiles.find_one({"_id": ctx.author.id})

        if not profile:
            raise cmd.BadArgument("No profile was found")
        if not config:
            raise cmd.BadArgument("No configuration was given")

        config = self.bot.utils.formatter(config, profile, ctx)

        _, embed = self.bot.utils.webhook_parser(config)
        embed = embed[0]

        await ctx.send(embed=embed)

    @cmd.command(name="Profile", aliases=['prf', 'профиль', "прф"], usage="профиль")
    @cmd.guild_only()
    async def profile(self, ctx: cmd.Context):
        config = await self.bot.config.find_one({"_id": self.bot.db_enumeration.get("profile")})
        profile = await self.profiles.find_one({"_id": ctx.author.id})

        if not profile:
            raise cmd.BadArgument("No profile was found")
        if not config:
            raise cmd.BadArgument("No configuration was given")

        config = self.bot.utils.formatter(config, profile, ctx)
        _, embed = self.bot.utils.webhook_parser(config)
        embed = embed[0]

        await ctx.send(embed=embed)

    @cmd.command(name="Reputation", aliases=['rep', "реп", "репутация"], usage="реп <user>")
    @cmd.guild_only()
    @cmd.cooldown(type=cmd.BucketType.user, per=86400, rate=1)
    async def reputation(self, ctx: cmd.Context, user: discord.Member):
        config = await self.bot.config.find_one({"_id": self.bot.db_enumeration.get("rep")})
        profile = await self.profiles.find_one({"_id": str(user.id)})

        if not profile:
            raise cmd.BadArgument("No profile was found")
        if not config:
            raise cmd.BadArgument("No configuration was given")

        profile['data']['rep'] += 1
        config = self.bot.utils.formatter(config, profile, ctx)
        _, embed = self.bot.utils.webhook_parser(config)
        embed = embed[0]

        await self.bot.profiles.update_one({"_id": str(ctx.author.id)}, {"$set": profile})
        await ctx.send(embed=embed)

    @cmd.command(name="Timely", aliases=['bonus', "free", "халява", "дань"], usage="дань")
    @cmd.guild_only()
    @cmd.cooldown(type=cmd.BucketType.user, per=86400, rate=1)
    async def timely(self, ctx: cmd.Context):
        ctx.author.tag = str(ctx.author)
        config = await self.bot.config.find_one({"_id": self.bot.db_enumeration.get("timely")})
        profile = await self.profiles.find_one({"_id": str(ctx.author.id)})

        if not profile:
            raise cmd.BadArgument("No profile was found")
        if not config:
            raise cmd.BadArgument("No configuration was given")

        profile['data']['hearts'] += 25
        config = self.bot.utils.formatter(config, profile, ctx)
        _, embed = self.bot.utils.webhook_parser(config)
        embed = embed[0]

        await self.profiles.update_one({"_id": str(ctx.author.id)}, {"$set": profile})
        await ctx.send(embed=embed)

    @cmd.command(name="Pay", aliases=['give', "дать"], usage="дать <user> <amount 1-10000>")
    @cmd.guild_only()
    async def pay(self, ctx: cmd.Context, user: discord.Member, amount: int):
        if amount not in range(1, 10000):
            raise cmd.BadArgument("Amount is out of range")

        ctx.author.tag = str(ctx.author)
        config = await self.bot.config.find_one({"_id": self.bot.db_enumeration.get("pay")})
        profile_author = await self.profiles.find_one({"_id": str(ctx.author.id)})
        profile_user = await self.profiles.find_one({"_id": str(user.id)})

        if not profile_user or not profile_author:
            raise cmd.BadArgument("No profiles was found")
        if not config:
            raise cmd.BadArgument("No configuration was given")
        if profile_author['data']['hearts'] < amount:
            raise cmd.BadArgument("Wallet value is less than amount")

        profile_author['data']['hearts'] -= amount
        profile_user['data']['hearts'] += amount
        await self.profiles.update_one({"_id": str(ctx.author.id)}, {"$set": profile_author})
        await self.profiles.update_one({"_id": str(user.id)}, {"$set": profile_user})

        for k, v in profile_author.items():
            profile_author.pop(k)
            profile_author[k + "_author"] = v

        for k, v in profile_user.items():
            profile_user.pop(k)
            profile_user[k + "_target"] = v

        profile = profile_author
        profile.update(profile_user)

        config = self.bot.utils.formatter(config, profile, ctx)
        _, embed = self.bot.utils.webhook_parser(config)
        embed = embed[0]

        await ctx.send(embed=embed)


def setup(bot):
    bot.add_cog(Economics(bot))
