import sys
import numpy as np

import socket
import os
import sys

import io
import json

from ule.util import jsonparser

def load(name='none', connectToRunning=True, port=3000):
    if not connectToRunning:
        if start(name, tcpport):
            print('successfully started environment %s.'%name)
        else:
            print('could not start environment %s'%name)
            return None
    env = Env(port=port)
    env.connect()
    return env

def start(name, tcpport):
    if find_executable(name):
        return start_executable(name, tcpport)    
    elif build_executable(name):
        return start_executable(name, tcpport)
    return False

def start_executable(name, port):
    return False

def find_executable(exe_name):
    return False

def build_executable(exe_name):
    return False

class Env(object):
    def __init__(self, hostIP="127.0.0.1", port=3000):
        self.__closed = False
        
        self.__motors = None
        self.__sensors = None

        self.__hostIP = "127.0.0.1"
        self.__port = port
    
    def connect(self):
        self.__sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.__sock.connect((self.__hostIP, self.__port))

        # get environment info
        msg = {}
        msg["method"] = "getEnvironmentInfo"
        self.__sock.send(json.dumps(msg))
        info = self.__sock.recv(16384)

        self.__sensors, self.__motors = jsonparser.parseSensorsAndMotors(info)

    def step(self):
        #send action
        jsonmotors = jsonparser.motorsToJson(self.__motors)
        self.__sock.send(jsonmotors)

        #receive sensor, reward and other info from unity environment
        feedback = self.__sock.recv(16384)
        reward, done, info = jsonparser.parseFeedback(feedback, self.__sensors)
        return reward, done, info


    def reset(self):
        reset = {}
        reset['method'] = 'reset'
        self.__sock.send(json.dumps(reset))
        feedback = self.__sock.recv(16384)
        jsonparser.parseFeedback(feedback, self.__sensors)


    def close(self):
        self.__sock.close()
        self.__closed = True

    def seed(self, seed=None):
        return 0

    def sensors(self):
        return self.__sensors
    
    def motors(self):
        return self.__motors

    def __del__(self):
        self.close()

    def __str__(self):
        return 'Env'
