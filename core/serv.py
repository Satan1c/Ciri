from flask import Flask
from multiprocessing import Process

app = Flask(__name__)

@app.route("/", methods=['GET', 'POST'])
def hello():
	return "bumped"

def f():
    Process(target=app.run).start()