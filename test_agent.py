import numpy as np
import ule

def main():
    env = ule.load()
    sensors = env.sensors()
    motors = env.motors()

    for i in range(500):
        for motor in motors:
            motor.randomize()

        reward, done, info = env.step()

    env.close()

if __name__ == '__main__':
    main()