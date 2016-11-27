import numpy as np
import ule

import time

def main():
    env = ule.load()
    sensors = env.sensors()
    motors = env.motors()

    for game in range(10):
        done = 0
        env.reset()
        while done == 0:
            for motor in motors:
                motor.randomize()

            reward, done, info = env.step()
            print(reward)

    env.close()

if __name__ == '__main__':
    main()