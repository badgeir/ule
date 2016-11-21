
# Author: Peter Leupi
# UDP interface between Unity learning environment and python

import numpy as np

from matplotlib import image as mpimg

import socket
import os
import sys

import io
import json

class ULEIface:
    def __init__(self, hostIP="127.0.0.1", actionPort=3000, imagePort=3001, infoPort=3002):
        self.hostIP = "127.0.0.1"
        self.actionPort = actionPort
        self.imagePort = imagePort
        self.infoPort = infoPort
        
        self.actionSock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.actionSock.connect((self.hostIP, self.actionPort))

        self.imageSock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.imageSock.connect((self.hostIP, self.imagePort))

        self.infoSock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.infoSock.bind((self.hostIP, self.infoPort))
        
        # wait for handshake
        self.infoSock.recvfrom(1024)

    def step(self, action):
        #send action
        self.actionSock.send(str(action))

        #receive image from environment
        imgbytes = self.imageSock.recv(10000)
        
        #receive other info from environment
        infostr, addr = self.infoSock.recvfrom(1024)

        img = None
        try:
            img = mpimg.imread(io.BytesIO(imgbytes))
        except Exception as e:
            print('not an image')

        info = self.decodeJson(json.loads(infostr))

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
        status = info['status']
        return observation, reward, status

    def decodeJson(self, info):
        decoded = {}
        for key in info.keys():
            k = str(key)

            if info[k].has_key('dtype'):
                dtype = str(info[k]['dtype'])
                if dtype == 'int':
                    decoded[k] = int(info[k]['value'])
                elif dtype == 'float':
                    decoded[k] = float(info[k]['value'])
            elif k == 'observation':
                decoded[k] = self.decodeJson(info[k])
        return decoded

    
    def close(self):
        self.actionSock.close()