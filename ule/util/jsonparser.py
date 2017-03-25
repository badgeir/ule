
import json
import numpy as np

from ule.sensorimotor import Sensor, Motor
from ule.spaces import Discrete, Vector, Image


def parseFeedback(string, sensors):
    jsonlst = json.loads(string)

    if 'sensors' in jsonlst:
        # modify sensors directly
        updateSensors(jsonlst['sensors'], sensors)

    reward = int(jsonlst['reward'])
    done = int(jsonlst['done'])

    info = jsonlst['info']

    return reward, done, info


def updateSensors(jsonsensors, sensors):
    for jsensor in jsonsensors:
        for sensor in sensors:
            if sensor.name() == jsensor['name']:
                sensor.value_from_json(jsensor['value'])


def parseSensorsAndMotors(info):
    jsn = json.loads(info)

    jsonsensors = {}
    jsonmotors = {}

    if 'sensors' in jsn:
        jsonsensors = jsn['sensors']
    if 'motors' in jsn:
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


def motorsToJsonable(motors):
    motorlist = []
    for i in range(len(motors)):
        current = motors[i].to_jsonable()
        motorlist.append(current)

    return motorlist
