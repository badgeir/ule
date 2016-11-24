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
        self.actionSpace = None
        self.observationSpace = None

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
        self.observationSpace, self.actionSpace = jsonparser.decodeSpaces(info)

    def step(self, action):
        #send action
        self.actionSock.send(str(action))

        #receive image from environment
        imgbytes = self.imageSock.recv(10000)

        #receive other info from environment
        infostr = self.infoSock.recv(1024)

        img = None
        try:
            img = mpimg.imread(io.BytesIO(imgbytes))
        except Exception as e:
            print('not an image')
        
        # decode json string
        info = jsonparser.decodeJsonStr(infostr)

        # package image and additional observations (if they exist) in a struct
        observation = {}
        observation['img'] = img
        if info.has_key('observation'):
            for key in info['observation'].keys():
                observation[key] = info['observation'][key]
            del info['observation']

        if not info.has_key('reward') or not info.has_key('status'):
            print('Error: incomplete data received')
            sys.exit()
 
        reward = info['reward']
        done = info['status']
        return observation, reward, done

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

    def observation_space(self):
        return self.observationSpace
    
    def action_space(self):
        return self.actionSpace

    def __del__(self):
        self.close()

    def __str__(self):
        return 'Env'
