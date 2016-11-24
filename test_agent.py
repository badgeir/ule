import numpy as np
import ule
from ule.util import jsonparser

def main():
    env = ule.load()
    sensors = env.sensors()
    motors = env.motors()

    for i in range(500):

        for motor in motors:
            #motor.randomize()
            motor.set_value(motor.space().sample())
        
        image, sensors, reward, done, info = env.step(motors[0].value())

        for sensor in sensors:
            print('%s: %s'%(sensor.name(), str(sensor.value())))
   
    env.close()

if __name__ == '__main__':
    main()