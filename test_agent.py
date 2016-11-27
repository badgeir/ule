import numpy as np
import ule

import time

def main():
    env = ule.load()
    sensors = env.sensors()
    motors = env.motors()

    for i in range(10):
        for motor in motors:
            motor.randomize()

        reward, done, info = env.step()
        time.sleep(0.5)

    env.reset()

    for i in range(10):
        for motor in motors:
            motor.randomize()

        reward, done, info = env.step()
        time.sleep(0.5)

    env.close()

if __name__ == '__main__':
    main()