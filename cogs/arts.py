from typing import Union, Tuple, List

import discord
from discord.ext import commands as cmd


class Arts(cmd.Cog):
    def __init__(self, bot):
        self.bot = bot
        self.emoji = bot.get_emoji(779022997331378196) or "<:ftvalyta:779022997331378196>"

    async def post_to(self, url_or_file: Union[List[Union[str, discord.Attachment]], str, discord.Attachment], channel: Union[int, discord.TextChannel], ctx: cmd.Context) -> List[discord.Message]:
        url = discord.Embed.Empty if not url_or_file else url_or_file if isinstance(url_or_file, str) else url_or_file.url if isinstance(url_or_file, discord.Attachment) else list(url_or_file) if isinstance(url_or_file, tuple) and isinstance(url_or_file[0], str) else [i.url for i in url_or_file]
        channel = None if not channel else channel if isinstance(channel, discord.TextChannel) else self.bot.get_channel(channel)

        embed = discord.Embed(title=str(ctx.author),
                              colour=discord.Colour.dark_grey())
        msgs = []

        if isinstance(url, str):
            embed.set_image(url=url)
        elif isinstance(url, list):
            for i in url:
                embed.set_image(url=i)
                msg = await channel.send(embed=embed)
                msgs.append(msg)

            return msgs
        else:
            raise cmd.BadArgument("Invalid image url")

        msg = await channel.send(embed=embed)
        msgs.append(msg)
        return msgs

    @cmd.command(name="Anime post", aliases=["anime_post", "animepost", "аниме_пост", "анимепост", "ап", "ap"])
    @cmd.guild_only()
    async def _anime(self, ctx: cmd.Context, *urls):
        channel = ctx.guild.get_channel(684011863155146777) or 684011863155146777
        msgs = await self.post_to(urls if urls and len(urls) >= 2 else urls[0] if urls and len(
            urls) >= 1 else ctx.message.attachments if ctx.message.attachments and len(
            ctx.message.attachments) >= 2 else ctx.message.attachments[0],
                                   channel,
                                   ctx)
        for i in msgs:
            await i.add_reaction(emoji=self.emoji)

    @cmd.command(name="Art post", aliases=["art_post", "artpost", "арт_пост", "артпост", "арп", "arp"])
    @cmd.guild_only()
    async def _art(self, ctx: cmd.Context, *urls):
        channel = ctx.guild.get_channel(703569657390694410) or 703569657390694410
        msgs = await self.post_to(urls if urls and len(urls) >= 2 else urls[0] if urls and len(
            urls) >= 1 else ctx.message.attachments if ctx.message.attachments and len(
            ctx.message.attachments) >= 2 else ctx.message.attachments[0],
                                   channel,
                                   ctx)

        for i in msgs:
            await i.add_reaction(emoji=self.emoji)

    @cmd.command(name="Meme post", aliases=["meme_post", "memepost", "мем_пост", "мемпост", "мп", "mp"])
    @cmd.guild_only()
    async def _meme(self, ctx: cmd.Context, *urls):
        channel = ctx.guild.get_channel(691573782153789540) or 691573782153789540
        msgs = await self.post_to(urls if urls and len(urls) >= 2 else urls[0] if urls and len(
            urls) >= 1 else ctx.message.attachments if ctx.message.attachments and len(
            ctx.message.attachments) >= 2 else ctx.message.attachments[0],
                                   channel,
                                   ctx)

        for i in msgs:
            await i.add_reaction(emoji=self.emoji)

    @cmd.command(name="NSFW post", aliases=["ero_post", "eropost", "еро_пост", "эро_пост", "еропост", "эропост", "ep", "еп", "эп"])
    @cmd.guild_only()
    async def _nsfw(self, ctx: cmd.Context, *urls):
        channel = ctx.guild.get_channel(688082279314096211) or 688082279314096211
        msgs = await self.post_to(urls if urls and len(urls) >= 2 else urls[0] if urls and len(
            urls) >= 1 else ctx.message.attachments if ctx.message.attachments and len(
            ctx.message.attachments) >= 2 else ctx.message.attachments[0],
                                   channel,
                                   ctx)

        for i in msgs:
            await i.add_reaction(emoji=self.emoji)


def setup(bot):
    bot.add_cog(Arts(bot))
