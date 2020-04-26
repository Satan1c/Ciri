from core.bot import bot
from flask import Flask
from multiprocessing import Process

app = Flask(__name__)

@app.route("/")
def hello():
  return "bumped"

if __name__ == "__main__":
  bot.startup()
  Process(target=app.run()).start()
  