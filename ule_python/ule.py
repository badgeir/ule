
# Author: Peter Leupi
# TCP interface between Unity learning environment and python

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

        self.infoSock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.infoSock.connect((self.hostIP, self.infoPort))
        
        # wait for handshake
        motors = self.infoSock.recv(1024)
        self.motorsJson = json.loads(motors)["Motors"]

    def action_space(self):
        return self.availableActions()

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
        info = self.decodeJson(json.loads(infostr))

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
        status = info['status']
        return observation, reward, status

    def availableActions(self):
        actions = None
        motors = self.motorsJson
        try:
            for key in motors.keys():
                if motors[key]["type"] == 'Discrete Action':
                    actions = np.arange(int(motors[key]["range"]))
        except Exception as e:
            print('Error decoding available motors:',e)
            print(motors)
        return actions
    
    def decodeJson(self, info):
        #print(info)
        decoded = {}
        try:
            for key in info.keys():
                k = str(key)

                if info[k].has_key('datatype'):
                    dtype = str(info[k]['datatype'])
                    if dtype == 'int':
                        decoded[k] = int(info[k]['value'])
                    elif dtype == 'float' or dtype == 'System.Single':
                        decoded[k] = float(info[k]['value'])
                elif k == 'observation':
                    decoded[k] = self.decodeJson(info[k])
        except Exception as e:
            print('Error decoding json string:', e)
            print(info)
        return decoded

    
    def close(self):
        self.actionSock.close()
        self.imageSock.close()
        self.infoSock.close()