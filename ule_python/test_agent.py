import numpy as np
import time
import sys

from matplotlib import pyplot as plt
import ule

def main():
    env = ule.ULEIface()
    
    observation, reward, status = env.step(1)

    for k in observation.keys():
        print(k + ':')
        print(observation[k])
    
    
    print(reward)
    print(status)
    
    img = observation['img']
    plt.imshow(img)
    plt.show()

    env.close()

if __name__ == '__main__':
    main()