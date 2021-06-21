from typing import Union, List

import discord


class User:
    def __init__(self, data: dict = None, *, mem: discord.Member = None):
        if not data:
            data = User.__get(mem)

        self._id: int = data["_id"]
        self.tag: str = data["tag"]
        self.hearts: int = data["hearts"]
        self.donuts: int = data["donuts"]
        self.reps: int = data["reps"]
        self.voice_time: float = data["voice_time"]
        self.join_voice = data["join_voice"]
        self.lover = data["lover"]

    def __dict__(self):
        return {
            "_id": self._id,
            "tag": self.tag,
            "hearts": self.hearts,
            "donuts": self.donuts,
            "reps": self.reps,
            "voice_time": self.voice_time,
            "join_voice": self.join_voice,
            "lover": self.lover
        }

    @classmethod
    def __get(cls, target: discord.Member):
        return {
            "_id": target.id,
            "tag": str(target),
            "hearts": 0,
            "donuts": 0,
            "reps": 0,
            "voice_time": 0.0,
            "join_voice": None,
            "lover": None
        }

    @classmethod
    def get_data(cls, target: Union[discord.Member, List[discord.Member]]) -> List[dict]:
        return [cls.__get(target)] if isinstance(target, discord.Member) else [cls.__get(i) for i in target]
