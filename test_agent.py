import numpy as np
import ule
from ule.util import jsonparser

def main():
    env = ule.load()
    sensors = env.sensors()
    motors = env.motors()

    for i in range(500):

        action = motors[0].sample()
        image, sensors, reward, done, info = env.step(action)

        for sensor in sensors:
            print('%s: %s'%(sensor.name(), str(sensor.value())))
   
    env.close()

if __name__ == '__main__':
    main()