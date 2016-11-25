import sys
import numpy as np
from matplotlib import image as mpimg

import socket
import os
import sys

import io

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
    def __init__(self, hostIP="127.0.0.1", actionPort=3000, imagePort=3001, infoPort=3002):
        self.closed = False
        self.configured = False
        
        self.reward_range = (-np.inf, np.inf)
        self._motors = None
        self._sensors = None

        self.hostIP = "127.0.0.1"
        self.actionPort = actionPort
        self.imagePort = imagePort
        self.infoPort = infoPort
    
    def connect(self):
        self.actionSock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.actionSock.connect((self.hostIP, self.actionPort))

        self.imageSock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.imageSock.connect((self.hostIP, self.imagePort))

        self.infoSock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.infoSock.connect((self.hostIP, self.infoPort))

        # wait for handshake
        info = self.infoSock.recv(1024)
        self._sensors, self._motors = jsonparser.parseSensorsAndMotors(info)

    def step(self, motors):
        #send action
        #jsonmotors = jsonparser.parseMotors(motors)

        self.actionSock.send(str(motors[0].value()))

        #receive image from environment
        imgbytes = self.imageSock.recv(10000)

        #receive other info from environment
        feedback = self.infoSock.recv(1024)

        img = None
        try:
            img = mpimg.imread(io.BytesIO(imgbytes))
        except Exception as e:
            print('not an image')
        
        reward, done, info = jsonparser.parseFeedback(feedback, self._sensors)
        return img, self._sensors, reward, done, info


    def reset(self):
        observation = 0
        return observation

    def close(self):
        self.actionSock.close()
        self.imageSock.close()
        self.infoSock.close()
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
