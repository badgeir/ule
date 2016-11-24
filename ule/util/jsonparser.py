
import json
import numpy as np

from ule.spaces import Discrete, Vector

def decodeFeedback(string):
    jsonlst = json.loads(string)
    
    observation = decodeObservations(jsonlst['sensors'])

    reward = int(jsonlst['reward'])
    done = int(jsonlst['done'])
    
    info = jsonlst['info']

    return observation, reward, done, info

def decodeObservations(sensors):
    observations = {}

    for name in sensors.keys():
        if sensors[name]['type'] == 'vector':
            value_str_lst = sensors[name]['value']
            value = np.array(value_str_lst).astype('float')
            observations[str(name)] = value
        else:
            raise Exception('Unknown sensor type')
    return observations

def decodeSpaces(info):
    jsn = json.loads(info)
    observationspace = {}
    actionspace = {}

    sensors = jsn['sensors']
    motors = jsn['motors']

    n_obs = 0
    for name in sensors:
        sensortype = sensors[name]['type']
        if sensortype == 'vector':
            length = int(sensors[name]['length'])
            minval = float(sensors[name]['min'])
            maxval = float(sensors[name]['max'])
            observationspace[str(name)] = Vector(minval, maxval, length, name)
        else:
            raise Exception('Unknown sensor type')

    for name in motors:
        motortype = motors[name]['type']
        if motortype == 'vector':
            length = int(motors[name]['length'])
            minval = float(motors[name]['min'])
            maxval = float(motors[name]['max'])
            actionspace[str(name)] = Vector(minval, maxval, length, name)
        if motortype == 'discrete':
            rang = int(motors[name]['range'])
            actionspace[str(name)] = Discrete(rang, name)
        else:
            raise Exception('Unknown motor type')
    
    return observationspace, actionspace