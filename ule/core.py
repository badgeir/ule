import socket
import json
import subprocess
import os
import sys

from ule.util import jsonparser

import time
import logging


class Env(object):

    def __init__(self, unity_project_path='', name='', connect_to_running=False, host_ip="127.0.0.1", port=3000):

        logging.basicConfig(filename='ule_log.txt', level=logging.DEBUG)
        self._log = logging.getLogger(__name__)
        self._log.debug('Initializing Environment.')

        self._unity_path = unity_project_path
        self._name = name

        self.motors = None
        self.sensors = None

        self._hostIP = host_ip
        self._port = port

        self.__sock = None
        self._connected = False

        self._instance = None

        if not connect_to_running:
            if self._start_instance():
                self._log.debug('successfully started environment %s.' % name)
            else:
                self._log.debug('could not start environment %s' % name)
        self._connect()

    def _start_instance(self):
        path_to_exe = os.path.join(self._unity_path, "Deploy", self._name)
        self._instance = subprocess.Popen([path_to_exe, '-port=%d' % self._port])
        if self._instance:
            self._log.debug('Started exe %s'%self._name)
            return True
        else:
            self._log.debug('Failed to start exe %s'%self._name)
            return False

    def _connect(self):
        self._log.debug('Connecting to ULE server...')
        self.__sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        while self._connected == False:
            try:
                self.__sock.connect((self._hostIP, self._port))
                self._connected = True
            except Exception as e:
                time.sleep(0.5)

        # get environment info
        self._log.debug('Getting environment info.')
        msg = {"method": "env"}
        self.__sock.send(json.dumps(msg).encode('utf-8'))
        info = self.__sock.recv(16384).decode('utf-8')

        self.sensors, self.motors = jsonparser.parseSensorsAndMotors(info)

    def step(self):
        self._log.debug('stepping')
        # send and receive environment update
        rpc = {}
        rpc['method'] = 'step'
        rpc['parameters'] = {}
        rpc['parameters']['motors'] = jsonparser.motorsToJsonable(self.motors)
        self.__sock.send(json.dumps(rpc).encode('utf-8'))

        self._log.debug('sent update')
        # receive sensor, reward and other info from unity environment
        feedback = self.__sock.recv(16384).decode('utf-8')
        self._log.debug('reveived environment update.')
        reward, done, info = jsonparser.parseFeedback(feedback, self.sensors)
        self._log.debug('parsed environment feedback.')
        return reward, done, info

    def reset(self):
        self._log.debug('resetting environment.')
        reset = {'method': 'reset'}
        self.__sock.send(json.dumps(reset).encode('utf-8'))
        feedback = self.__sock.recv(16384).decode('utf-8')
        jsonparser.parseFeedback(feedback, self.sensors)
        self._log.debug('Environment reset.')

    def close(self):
        self._log.debug('closing environment.')
        self.__sock.close()
        self._connected = False
        if self._instance != None:
            self._instance.terminate()

    @staticmethod
    def seed(seed=None):
        import ule.spaces
        if seed:
            ule.spaces.seed(seed)

    def __del__(self):
        self.close()

    def __str__(self):
        return 'Env'
