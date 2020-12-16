from typing import Union, List

import discord


class User:
    @classmethod
    def __get(cls, target: discord.Member):
        return {
            "_id": target.id,
            "tag": str(target),
            "hearts": 0,
            "donuts": 0,
            "reps": 0,
            "voice_time": 0.0,
            "lover": None
        }

    @classmethod
    def get_data(cls, target: Union[discord.Member, List[discord.Member]]) -> List[dict]:
        return [cls.__get(target)] if isinstance(target, discord.Member) else [cls.__get(i) for i in target]
