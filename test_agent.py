import numpy as np
import ule

def main():
    env = ule.load()
    sensors = env.sensors()
    motors = env.motors()

    print(motors)

    for i in range(100):
        for motor in motors:
            motor.randomize()

        reward, done, info = env.step()

        for sensor in sensors:
            print('%s: %s'%(sensor.name(), str(sensor.value())))
   
    env.close()

if __name__ == '__main__':
    main()