from core.bot import bot
from flask import Flask
from multiprocessing import Process

app = Flask(__name__)

@app.route("/")
def hello():
  return "bumped"

def f():
  app.run()

if __name__ == "__main__":
  Process(target=f()).start()
  bot.startup()
  