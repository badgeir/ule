import socket
import json
import subprocess
import os

from ule.util import jsonparser
import time


class Env(object):

    def __init__(self, unity_project_path='', name='', connect_to_running=False, host_ip="127.0.0.1", port=3000):

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
                print('successfully started environment %s.' % name)
            else:
                print('could not start environment %s' % name)
        self._connect()

    def _start_instance(self):
        path_to_exe = os.path.join(self._unity_path, "Deploy", self._name)
        print("whuuut")
        print(path_to_exe)
        self._instance = subprocess.Popen([path_to_exe, '-port=%d' % self._port])
        if self._instance:
            return True
        else:
            return False

    def _connect(self):
        self.__sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        while self._connected == False:
            try:
                self.__sock.connect((self._hostIP, self._port))
                self._connected = True
            except Exception as e:
                time.sleep(0.5)
            
        # get environment info
        msg = {"method": "getEnvironmentInfo"}
        self.__sock.send(json.dumps(msg))
        info = self.__sock.recv(16384)

        self.sensors, self.motors = jsonparser.parseSensorsAndMotors(info)

    def step(self):
        # send action
        jsonmotors = jsonparser.motorsToJson(self.motors)
        self.__sock.send(jsonmotors)

        # receive sensor, reward and other info from unity environment
        feedback = self.__sock.recv(16384)
        reward, done, info = jsonparser.parseFeedback(feedback, self.sensors)
        return reward, done, info

    def reset(self):
        reset = {'method': 'reset'}
        self.__sock.send(json.dumps(reset))
        feedback = self.__sock.recv(16384)
        jsonparser.parseFeedback(feedback, self.sensors)

    def close(self):
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
