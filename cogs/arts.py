from typing import Union, Tuple, List

import discord
from discord.ext import commands as cmd


class Arts(cmd.Cog):
    def __init__(self, bot):
        self.bot = bot
        self.ero_emoji = bot.get_emoji(781849766610731020) or "<a:firegife:781849766610731020>"
        self.meme_emoji = bot.get_emoji(781215691272355893) or "<:pepeFT:781215691272355893>"
        self.art_emoji = bot.get_emoji(769546004373831720) or "<a:391:769546004373831720>"
        self.channels = {
            703569657390694410: {
                "colour": (47, 50, 54,),
                "emoji": self.art_emoji
            },
            691573782153789540: {
                "colour": (47, 50, 54,),
                "emoji": self.meme_emoji
            },
            688082279314096211: {
                "colour": (212, 0, 255,),
                "emoji": self.ero_emoji
            }
        }

    async def post_to(self, url_or_file: Union[List[Union[str, discord.Attachment]], str, discord.Attachment], channel: Union[int, discord.TextChannel], ctx: cmd.Context) -> List[discord.Message]:
        url = discord.Embed.Empty if not url_or_file else url_or_file if isinstance(url_or_file, str) else url_or_file.url if isinstance(url_or_file, discord.Attachment) else list(url_or_file) if isinstance(url_or_file, tuple) and isinstance(url_or_file[0], str) else [i.url for i in url_or_file]
        channel = None if not channel else channel if isinstance(channel, discord.TextChannel) else self.bot.get_channel(channel)

        embed = discord.Embed(title=str(ctx.author),
                              colour=discord.Colour.from_rgb(*self.channels[channel.id]['colour']))
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

    # @cmd.command(name="Anime post", aliases=["anime_post", "animepost", "аниме_пост", "анимепост", "ап", "ap"])
    # @cmd.guild_only()
    # async def _anime(self, ctx: cmd.Context, *urls):
    #     channel = ctx.guild.get_channel(684011863155146777) or 684011863155146777
    #     msgs = await self.post_to(urls if urls and len(urls) >= 2 else urls[0] if urls and len(
    #         urls) >= 1 else ctx.message.attachments if ctx.message.attachments and len(
    #         ctx.message.attachments) >= 2 else ctx.message.attachments[0],
    #                                channel,
    #                                ctx)
    #     for i in msgs:
    #         await i.add_reaction(emoji=self.emoji)

    @cmd.command(name="Art post", aliases=["art_post", "artpost", "арт_пост", "артпост", "ап", "ap"])
    @cmd.guild_only()
    async def _art(self, ctx: cmd.Context, *urls):
        channel = ctx.guild.get_channel(703569657390694410) or 703569657390694410
        msgs = await self.post_to(urls if urls and len(urls) >= 2 else urls[0] if urls and len(
            urls) >= 1 else ctx.message.attachments if ctx.message.attachments and len(
            ctx.message.attachments) >= 2 else ctx.message.attachments[0],
                                   channel,
                                   ctx)

        for i in msgs:
            await i.add_reaction(emoji=self.art_emoji)

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
            await i.add_reaction(emoji=self.meme_emoji)

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
            await i.add_reaction(emoji=self.ero_emoji)


def setup(bot):
    bot.add_cog(Arts(bot))
