import sys
import numpy as np
from matplotlib import image as mpimg

import socket
import os
import sys

import io

import json

import ule
from ule.util import jsonparser
from ule.spaces import Vector

def load(name='none', connectToRunning=True):
    if not connectToRunning:
        if start(name):
            print('successfully started environment %s.'%name)
    
    env = Env()
    env.connect()
    return env

def start(name):
    return False

class Env(object):
    def __init__(self, hostIP="127.0.0.1", port=3000):
        self.closed = False
        self.configured = False
        
        self.reward_range = (-np.inf, np.inf)
        self._motors = None
        self._sensors = None

        self.hostIP = "127.0.0.1"
        self.port = port
    
    def connect(self):
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.sock.connect((self.hostIP, self.port))

        # get environment info
        msg = {}
        msg["method"] = "getEnvironmentInfo"
        self.sock.send(json.dumps(msg))

        info = self.sock.recv(1024)
        self._sensors, self._motors = jsonparser.parseSensorsAndMotors(info)

    def step(self):
        
        #send action
        jsonmotors = jsonparser.motorsToJson(self._motors)
        self.sock.send(jsonmotors)

        #receive image from environment
        #imgbytes = self.imageSock.recv(10000)

        #receive other info from environment
        feedback = self.sock.recv(1024)

        # img = None
        # try:
        #     img = mpimg.imread(io.BytesIO(imgbytes))
        # except Exception as e:
        #     print('not an image')
        
        reward, done, info = jsonparser.parseFeedback(feedback, self._sensors)
        return reward, done, info


    def reset(self):
        observation = 0
        return observation

    def close(self):
        self.sock.close()
        self.closed = True

    def seed(self, seed=None):
        return 0

    def configure(self, *args, **kwargs):
        return False

    def sensors(self):
        return self._sensors
    
    def motors(self):
        return self._motors

    def __del__(self):
        self.close()

    def __str__(self):
        return 'Env'
