import numpy as np
import time
import sys

from matplotlib import pyplot as plt
import ule

def main():
    env = ule.load()
    
    # observation, reward, status = env.step(1)
    # print(observation)
    # print(reward)
    # print(status)
    starttime = time.clock()
    for i in range(500):
        observation, reward, status = env.step(np.random.randint(3))
        endtime = time.clock()
        print(endtime - starttime)
        starttime = endtime
    
    env.close()

if __name__ == '__main__':
    main()