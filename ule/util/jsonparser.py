
import json
import numpy as np

from ule.sensorimotor import Sensor, Motor
from ule.spaces import Discrete, Vector

def parseFeedback(string, sensors):
    jsonlst = json.loads(string)
    
    #modify sensors directly
    updateSensors(jsonlst['sensors'], sensors)

    reward = int(jsonlst['reward'])
    done = int(jsonlst['done'])
    
    info = jsonlst['info']

    return reward, done, info

def updateSensors(jsonsensors, sensors):
    for name in jsonsensors:
        for sensor in sensors:
            if sensor.name() == name:
                sensor.value_from_json(jsonsensors[name])

def parseSensorsAndMotors(info):
    jsn = json.loads(info)

    jsonsensors = jsn['sensors']
    jsonmotors = jsn['motors']

    sensors = []
    motors = []

    for jsonsensor in jsonsensors:
        try:
            sensors.append(Sensor(jsonsensor))
        except Exception as e:
            print('Error decoding json sensors.')

    for jsonmotor in jsonmotors:
        try:
            motors.append(Motor(jsonmotor))
        except Exception as e:
            print('Error decoding json motors.')
        
    
    return sensors, motors