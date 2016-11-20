
# Author: Peter Leupi
# UDP interface between Unity learning environment and python

import numpy as np

from matplotlib import pyplot as plt
from matplotlib import image as mpimg

import socket
import os
import sys

import io

class ULEIface:
    def __init__(self, hostIP="127.0.0.1", actionPort=3000, imagePort=3001, rewardPort=3002, statusPort=3003):
        self.hostIP = "127.0.0.1"
        self.actionPort = actionPort
        self.imagePort = imagePort
        self.rewardPort = rewardPort
        self.statusPort = statusPort

        self.actionSock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        
        self.imageSock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.imageSock.bind((self.hostIP, self.imagePort))

        self.rewardSock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.rewardSock.bind((self.hostIP, self.rewardPort))
    
        self.statusSock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.statusSock.bind((self.hostIP, self.statusPort))
        
        print('Created udp sockets.')

    def step(self, action):
        #send action
        self.actionSock.sendto(str(action), (self.hostIP, self.actionPort))

        #receive feedback from environment
        imgbytes, addr = self.imageSock.recvfrom(10000)
        rewardstr, addr = self.rewardSock.recvfrom(1024)
        statusstr, addr = self.statusSock.recvfrom(1024) 

        img = None
        try:
            img = mpimg.imread(io.BytesIO(imgbytes))
        except Exception as e:
            print('not an image')
        
        reward = float(rewardstr)
        status = int(statusstr)

        return img, reward, status
