import socket
import json

from ule.util import jsonparser


def load(name='none', connect_to_running=True, port=3000):
    if not connect_to_running:
        if start(name, port):
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
    def __init__(self, host_ip="127.0.0.1", port=3000):
        self._closed = False
        
        self.motors = None
        self.sensors = None

        self._hostIP = host_ip
        self._port = port

        self.__sock = None
    
    def connect(self):
        self.__sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.__sock.connect((self._hostIP, self._port))

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
        self._closed = True

    def seed(self, seed=None):
        import ule.spaces
        if seed:
            ule.spaces.seed(seed)

    def __del__(self):
        self.close()

    def __str__(self):
        return 'Env'
