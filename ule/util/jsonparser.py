
import json
import numpy as np

from ule.sensorimotor import Sensor, Motor
from ule.spaces import Discrete, Vector

import io
from matplotlib import image as mpimg

def parseFeedback(string, sensors):
    jsonlst = json.loads(string)
    
    #modify sensors directly
    updateSensors(jsonlst['sensors'], sensors)

    imgbytes = jsonlst['image'].decode('base64')

    img = None
    try:
        img = mpimg.imread(io.BytesIO(imgbytes))
    except Exception as e:
        print('not an image')

    reward = int(jsonlst['reward'])
    done = int(jsonlst['done'])
    
    info = jsonlst['info']

    return img, reward, done, info

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

def motorsToJson(motors):
    jmsg = {}

    jmsg['method'] = 'updateEnvironment'
    jmsg['parameters'] = {}
    jmsg['parameters']['motors'] = []

    for i in range(len(motors)):
        motorstr = motors[i].to_jsonable()
        jmsg['parameters']['motors'].append(motorstr)
    
    return json.dumps(jmsg)
