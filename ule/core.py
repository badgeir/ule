import sys
import numpy as np

import socket
import os
import sys

import io
import json

from ule.util import jsonparser

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
        info = self.sock.recv(16384)

        self._sensors, self._motors = jsonparser.parseSensorsAndMotors(info)

    def step(self):
        import time

        #send action
        jsonmotors = jsonparser.motorsToJson(self._motors)
        self.sock.send(jsonmotors)

        #receive other info from environment
        feedback = self.sock.recv(16384)
        reward, done, info = jsonparser.parseFeedback(feedback, self._sensors)
        return reward, done, info


    def reset(self):
        reset = {}
        reset['method'] = 'reset'
        self.sock.send(json.dumps(reset))
        feedback = self.sock.recv(16384)
        jsonparser.parseFeedback(feedback, self._sensors)


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
