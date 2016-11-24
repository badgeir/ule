
import json

from ule.spaces import Discrete, Vector

def decode(string, observationspace):
    observations = ()
    reward = 0
    done = 0
    info = ()

    jsonlst = json.loads(jsonlst)
    
    observation = decodeObservations(jsonlst['sensors'], observationspace)

    reward = int(jsonlst['reward'])
    done = int(jsonlst['done'])
    
    info = decodeInfo(jsonlst['info'])

    return observation, reward, done, info

def decodeSpaces(info):
    jsn = json.loads(info)
    observationspace = []
    actionspace = []

    sensors = jsn['sensors']
    motors = jsn['motors']

    n_obs = 0
    for name in sensors:
        sensortype = sensors[name]['type']
        if sensortype == 'vector':
            length = int(sensors[name]['length'])
            minval = float(sensors[name]['min'])
            maxval = float(sensors[name]['max'])
            observationspace.append(Vector(minval, maxval, length, name))
        else:
            raise Exception('Unknown sensor type')

    for name in motors:
        motortype = motors[name]['type']
        if motortype == 'vector':
            length = int(motors[name]['length'])
            minval = float(motors[name]['min'])
            maxval = float(motors[name]['max'])
            actionspace.append(Vector(minval, maxval, length, name))
        if motortype == 'discrete':
            rang = int(motors[name]['range'])
            actionspace.append(Discrete(rang, name))
        else:
            raise Exception('Unknown motor type')

    if(len(observationspace) == 1):
        observationspace = observationspace[0]
    if(len(actionspace) == 1):
        actionspace = actionspace[0]
    
    return observationspace, actionspace


def decodeObservations(sensors, observationspace):
    observations = {}
    name = observationspace.name()
    for key in sensors.keys():
        if sensors[key]['type'] == 'float':
            observations['key'] = float(sensors[key]['value'])
    
    return observations

def decodeInfo(jsonlst):
    return None

def decodeJsonStr(str):
    jsn = json.loads(str)
    return decodeJson(jsn)

def decodeJson(info):
    decoded = {}
    try:
        for key in info.keys():
            k = str(key)
            if info[k].has_key('type'):
                dtype = str(info[k]['type'])
                if dtype == 'int':
                    decoded[k] = int(info[k]['value'])
                elif dtype == 'float' or dtype == 'System.Single':
                    decoded[k] = float(info[k]['value'])
            elif k == 'observation':
                decoded[k] = decodeJson(info[k])
    
    except Exception as e:
        print('Error decoding json string:', e)
    return decoded