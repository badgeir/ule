import numpy as np
import time
import sys

from matplotlib import pyplot as plt
import ule

def main():
    env = ule.ULEIface()
    
    status = 0

    try:
        for i in range(10):
            observation, reward, status = env.step(0)
            img = observation['img']
            axis0 = observation['axis0']
            print (img.shape)
            print(axis0)
            print(reward)
            print(status)

    except Exception as e:
        sys.exit()
    
    env.close()

if __name__ == '__main__':
    main()