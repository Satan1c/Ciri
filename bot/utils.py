import ast
import re
from time import localtime
from typing import Union, Tuple, List

import discord
from discord.ext import commands as cmd
from motor.motor_asyncio import AsyncIOMotorClient, AsyncIOMotorCollection
from config import mongo

class Utils:
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

    @staticmethod
    def tm(string: str = ""):
        hour = str(localtime().tm_hour + 3)
        if int(hour) >= 24:
            hour = str(int(hour) - 24)
        minu = str(localtime().tm_min)
        tm = string + "[" + (hour if len(hour) == 2 else "0" + hour) + " : " + (
            minu if len(minu) == 2 else "0" + minu) + "]"

        return str(tm)

    # async def up_remind(self, channel: int, command: str):
    #     await asyncio.sleep(delay=4 * 60 * 60)
    #
    #     role = self.bot.get_guild(542005378049638400).get_role(709306526094983209)
    #     await self.bot.get_channel(channel).send(f":exclamation: {role.mention} время писать {command}")

    async def post_to(self, url_or_file: Union[List[Union[str, discord.Attachment]], str, discord.Attachment],
                      channel: Union[int, discord.TextChannel], ctx: cmd.Context) -> List[discord.Message]:

        urls = []
        if not url_or_file:
            urls.append(discord.Embed.Empty)

        elif isinstance(url_or_file, str) or isinstance(url_or_file, discord.Attachment):
            urls.append(url_or_file if isinstance(url_or_file, str) else url_or_file.url)

        else:
            for i in url_or_file:
                i = i if isinstance(i, str) else i.url
                urls.append(i)

        channel = None if not channel else channel if isinstance(channel,
                                                                 discord.TextChannel) else self.bot.get_channel(channel)

        embed = discord.Embed(colour=discord.Colour.from_rgb(*self.channels[channel.id]['colour']))
        msgs = []
        for url in urls:
            if isinstance(url, str):
                embed.set_author(name=str(ctx.author), icon_url=ctx.author.avatar_url_as(static_format='png', size=512))
                embed.set_image(url=url)
                msg = await channel.send(embed=embed)
                msgs.append(msg)
            else:
                raise cmd.BadArgument("Invalid image url")

        return msgs

    @staticmethod
    def webhook_parser(data: dict) -> Tuple[str, List[discord.Embed]]:
        message = ""
        embed = []

        if "content" in data:
            message = data['content']

        if "embeds" in data:
            if len(data['embeds']):
                embed = []
                for i in data['embeds']:
                    embed.append(discord.Embed().from_dict(dict(i)))

        if not len(embed):
            embed.append(discord.Embed())
        return message, embed

    def formatter(self, config: dict, profile: dict, ctx: cmd.Context) -> dict:
        for k, v in profile.items():
            if v is None:
                profile[k] = "нет"

        return ast.literal_eval(
            self.__reg_sub(
                self.__reg_sub(
                    str(config)).format(user=ctx.author,
                                        prefix=self.bot.command_prefix,
                                        ctx=ctx,
                                        **profile)))

    def __reg_sub(self, text: str) -> str:
        s = re.sub(r"'~", "'}", text)
        s = re.sub(r"]~", "]}", s)
        s = re.sub(r"~]", "}]", s)
        s = re.sub(r"None~", "None}", s)
        s = re.sub(r"True~", "True}", s)
        s = re.sub(r"~'", "{'", s)

        return s

class DataBaseAccess:
    def __init__(self):
        self.client = AsyncIOMotorClient(mongo)
        self.server = self.client.get_database("server")
        self.profiles = self.client.get_database("users").get_collection("profiles")
        self.config = self.client.get_database("cfg").get_collection("main")

    async def _find_one(self, collection: AsyncIOMotorCollection, query: dict, *, error: Exception = None) -> Union[dict, None]:
        doc: Union[dict, None] = await collection.find_one(query)

        if not doc and error:
            raise error

        return doc

    async def get_one_config(self, query: dict, can_none: bool = True, none_form: dict = None) -> Union[dict, None]:
        if not can_none and not none_form:
            return await self._find_one(self.config, query, error=cmd.BadArgument("No config found"))

        if not can_none:
            await self.config.insert_one(none_form)
            return none_form

        return await self._find_one(self.config, query)

    async def update_one_config(self, query: dict, data: dict) -> None:
        cfg = await self.config.update_one(query, data if "$set" in data else {"$set": data})

        if not cfg:
            query.update(data)
            await self.config.insert_one(query)

    async def delete_one_config(self, query: dict) -> None:
        cfg = await self.config.find_one_and_delete(query)

        if not cfg:
            raise cmd.BadArgument("No config found")

    async def get_one_profile(self, query: dict, can_none: bool = True, none_form: dict = None) -> Union[dict, None]:
        if not can_none and not none_form:
            return await self._find_one(self.profiles, query, error=cmd.BadArgument("No profile found"))

        prf = await self._find_one(self.profiles, query)

        if not prf and not can_none:
            await self.profiles.insert_one(none_form)
            return none_form

        return prf

    async def update_one_profile(self, query: dict, data: dict) -> None:
        cfg = await self.profiles.update_one(query, data if "$set" in data else {"$set": data})

        if not cfg:
            query.update(data)
            await self.profiles.insert_one(query)

    async def delete_one_profile(self, query: dict) -> None:
        cfg = await self.profiles.find_one_and_delete(query)

        if not cfg:
            raise cmd.BadArgument("No profile found")


class Paginator:
    def __init__(self, ctx: cmd.Context, begin: discord.Embed = None, timeout: int = 120,
                 embeds: Union[tuple, list] = None):
        self.reactions = ('⬅', '🪣', '➡')
        self.pages = []
        self.current = 0
        self.ctx = ctx
        self.timeout = timeout
        self.begin = begin
        self.add_page(embeds)
        self.controller = None

    async def _close_session(self):
        await self.controller.delete()
        del self.pages, self.reactions, self.current, self.ctx

    def add_page(self, embeds: Union[list, tuple]):
        for i in embeds:
            if isinstance(i, discord.Embed):
                self.pages.append(i)

    async def call_controller(self, start_page: int = 0):
        if start_page > len(self.pages) - 1:
            raise IndexError(f'Currently added {len(self.pages)} pages,'
                             f' but you tried to call controller with start_page = {start_page}')
        if not self.controller:
            self.controller = await self.ctx.send(embed=self.pages[start_page])
        else:
            await self.controller.edit(embed=self.pages[start_page])

        try:
            await self.controller.clear_reactions()
        except BaseException as err:
            print("\n", "-" * 30, f"\n[!]Paginator call_controller clear_reactions error:\n{err}\n", "-" * 30, "\n")
            pass

        try:
            for emoji in self.reactions:
                await self.controller.add_reaction(emoji)
        except BaseException as err:
            print("\n", "-" * 30, f"\n[!]Paginator call_controller add_reaction error:\n{err}\n", "-" * 30, "\n")
            return

        while True:
            try:
                def check(r, u) -> bool:
                    return u.id == self.ctx.author.id \
                           and r.emoji in self.reactions \
                           and r.message.id == self.controller.id

                response = await self.ctx.bot.wait_for('reaction_add', timeout=self.timeout,
                                                       check=check)
            except TimeoutError:
                break

            try:
                await self.controller.remove_reaction(response[0], response[1])
            except BaseException as err:
                print(err)
                pass

            if response[0].emoji == self.reactions[0]:
                self.current = self.current - 1 if self.current > 0 else len(self.pages) - 1
                await self.controller.edit(embed=self.pages[self.current])

            if response[0].emoji == self.reactions[1]:
                break

            if response[0].emoji == self.reactions[2]:
                self.current = self.current + 1 if self.current < len(self.pages) - 1 else 0
                await self.controller.edit(embed=self.pages[self.current])

        await self._close_session()
