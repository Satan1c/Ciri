from bot.bot import Ciri
from discord.ext import commands as cmd


class Arts(cmd.Cog):
    def __init__(self, bot: Ciri):
        self.bot = bot
        self.ero_emoji = bot.get_emoji(781849766610731020) or "<a:firegife:781849766610731020>"
        self.meme_emoji = bot.get_emoji(781215691272355893) or "<:pepeFT:781215691272355893>"
        self.art_emoji = bot.get_emoji(769546004373831720) or "<a:391:769546004373831720>"

    @cmd.command(name="Art post", aliases=["art_post", "artpost", "арт_пост", "артпост", "ап", "ap"])
    @cmd.guild_only()
    async def art(self, ctx: cmd.Context, *urls):
        channel = ctx.guild.get_channel(703569657390694410) or 703569657390694410

        urls = urls if urls and not ctx.message.attachments else \
            ctx.message.attachments if ctx.message.attachments and not urls else \
                list(urls) + ctx.message.attachments
        urls = urls if len(urls) >= 2 else urls[0] if len(urls) >= 1 else None

        msgs = await self.bot.utils.post_to(urls, channel, ctx)

        for i in msgs:
            await i.add_reaction(emoji=self.art_emoji)

    @cmd.command(name="Meme post", aliases=["meme_post", "memepost", "мем_пост", "мемпост", "мп", "mp"])
    @cmd.guild_only()
    async def meme(self, ctx: cmd.Context, *urls):
        channel = ctx.guild.get_channel(691573782153789540) or 691573782153789540

        urls = urls if urls and not ctx.message.attachments else \
            ctx.message.attachments if ctx.message.attachments and not urls else \
                list(urls) + ctx.message.attachments
        urls = urls if len(urls) >= 2 else urls[0] if len(urls) >= 1 else None

        msgs = await self.bot.utils.post_to(urls, channel, ctx)

        for i in msgs:
            await i.add_reaction(emoji=self.meme_emoji)

    @cmd.command(name="NSFW post",
                 aliases=["ero_post", "eropost", "еро_пост", "эро_пост", "еропост", "эропост", "ep", "еп", "эп"])
    @cmd.guild_only()
    async def nsfw(self, ctx: cmd.Context, *urls):
        channel = ctx.guild.get_channel(688082279314096211) or 688082279314096211

        urls = urls if urls and not ctx.message.attachments else \
            ctx.message.attachments if ctx.message.attachments and not urls else \
                list(urls) + ctx.message.attachments
        urls = urls if len(urls) >= 2 else urls[0] if len(urls) >= 1 else None

        msgs = await self.bot.utils.post_to(urls, channel, ctx)

        for i in msgs:
            await i.add_reaction(emoji=self.ero_emoji)


def setup(bot):
    bot.add_cog(Arts(bot))
