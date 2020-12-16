import discord
from bot.bot import Ciri
from discord.ext import commands as cmd


class Economics(cmd.Cog):
    def __init__(self, bot: Ciri):
        self.bot = bot
        self.profiles = bot.profiles

    @cmd.command(name="Profile", aliases=['prf', 'профиль', "прф"], usage="профиль")
    @cmd.guild_only()
    async def profile(self, ctx: cmd.Context):
        config = await self.bot.config.find_one()
        config = config['profile']
        profile = await self.profiles.find_one({"_id": str(ctx.author.id)})

        if not profile or (profile and "data" not in profile):
            raise cmd.BadArgument("No profile was found")

        data = profile['data']
        ctx.author.tag = str(ctx.author)
        cmds = [i.aliases[0] for j in self.bot.cogs for i in self.bot.cogs[j].walk_commands() if not i.hidden]
        config = str(config).format(user=ctx.author, data=data, prefix=self.bot.command_prefix, bot=self.bot.user,
                                    commands=cmds)

        _, embed = self.bot.utils.webhook_parser(config)
        embed = embed[0]

        await ctx.send(embed=embed)

    @cmd.command(name="Reputation", aliases=['rep', "реп", "репутация"], usage="rep <user>")
    @cmd.guild_only()
    @cmd.cooldown(type=cmd.BucketType.user, per=86400, rate=1)
    async def reputation(self, ctx: cmd.Context, user: discord.Member):
        config = await self.bot.config.find_one()
        config = config['rep']
        profile = await self.profiles.find_one({"_id": str(user.id)})

        if not profile or (profile and "data" not in profile):
            raise cmd.BadArgument("No profile was found")

        profile['data']['rep'] += 1
        await self.bot.profiles.update_one({"_id": str(ctx.author.id)}, {"$set": profile})

        ctx.author.tag = str(ctx.author)
        cmds = [i.aliases[0] for j in self.bot.cogs for i in self.bot.cogs[j].walk_commands() if not i.hidden]
        config = str(config).format(user=ctx.author, data=profile['data'], prefix=self.bot.command_prefix,
                                    bot=self.bot.user,
                                    commands=cmds)

        _, embed = self.bot.utils.webhook_parser(config)
        embed = embed[0]

        await ctx.send(embed=embed)

    @cmd.command(name="Timely", aliases=['bonus', "free", "халява", "дань"], usage="дань")
    @cmd.guild_only()
    @cmd.cooldown(type=cmd.BucketType.user, per=86400, rate=1)
    async def timely(self, ctx: cmd.Context):
        ctx.author.tag = str(ctx.author)
        config = await self.bot.config.find_one()
        config = config['timely']
        profile = await self.bot.profiles.find_one({"_id": str(ctx.author.id)})

        if not profile or (profile and "data" not in profile):
            raise cmd.BadArgument("No profile was found")

        profile['data']['hearts'] += 25
        await self.bot.profiles.update_one({"_id": str(ctx.author.id)}, {"$set": profile})

        cmds = [i.aliases[0] for j in self.bot.cogs for i in self.bot.cogs[j].walk_commands() if not i.hidden]
        config = str(config).format(user=ctx.author, data=profile['data'], prefix=self.bot.command_prefix,
                                    bot=self.bot.user,
                                    commands=cmds)

        _, embed = self.bot.utils.webhook_parser(config)
        embed = embed[0]

        await ctx.send(embed=embed)

    @cmd.command(name="Pay", aliases=['give', "дать"], usage="дать <user> <amount 1-10000>")
    @cmd.guild_only()
    async def pay(self, ctx: cmd.Context, user: discord.Member, amount: int):
        if amount not in range(1, 10000):
            raise cmd.BadArgument("Amount is out of range")

        ctx.author.tag = str(ctx.author)
        config = await self.bot.config.find_one()
        config = config['pay']
        profile_author = await self.bot.profiles.find_one({"_id": str(ctx.author.id)})
        profile_user = await self.bot.profiles.find_one({"_id": str(user.id)})

        if (not profile_user or not profile_author) or (profile_user and "data" not in profile_user) or (
                profile_author and "data" not in profile_author):
            raise cmd.BadArgument("No profiles was found")

        if profile_author['data']['hearts'] < amount:
            raise cmd.BadArgument("Wallet value is less than amount")

        profile_author['data']['hearts'] -= amount
        profile_user['data']['hearts'] += amount

        await self.bot.profiles.update_one({"_id": str(ctx.author.id)}, {"$set": profile_author})
        await self.bot.profiles.update_one({"_id": str(user.id)}, {"$set": profile_user})

        cmds = [i.aliases[0] for j in self.bot.cogs for i in self.bot.cogs[j].walk_commands() if not i.hidden]
        config = str(config).format(user=ctx.author, author_data=profile_author, target_data=profile_user,
                                    amount=amount, prefix=self.bot.command_prefix,
                                    bot=self.bot.user,
                                    commands=cmds)

        _, embed = self.bot.utils.webhook_parser(config)
        embed = embed[0]

        await ctx.send(embed=embed)


def setup(bot):
    bot.add_cog(Economics(bot))
