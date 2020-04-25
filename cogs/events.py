from flask import Flask
from multiprocessing import Process
from discord.ext import commands

app = Flask(__name__)

@app.route("/")
def hello():
  return "bumped"


    
class Events(commands.Cog):
  def __init__(self, bot):
    self.bot = bot
  
  def flask_start(self):
    app.run()
  
  @commands.listener()
  async def on_ready(self):
    Process(target=self.flask_start).start()
  
def setup(bot):
  bot.add_cog(Events(bot))