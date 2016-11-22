import numpy as np
import time
import sys

from matplotlib import pyplot as plt
import ule

def main():
    env = ule.ULEIface()
    
    observation, reward, status = env.step(1)

    for i in range(500):
        actions = env.action_space()
        act = np.random.choice(actions)
        observation, reward, status = env.step(act)
    
    env.close()

if __name__ == '__main__':
    main()