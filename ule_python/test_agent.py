import numpy as np
import time
import sys

from matplotlib import pyplot as plt
import ule

def main():
    env = ule.ULEIface()
    
    status = 0
    while status==0:
        try:
            img, reward, status = env.step(0)

            print (img.shape)
            print(reward)
            print(status)

        except Exception as e:
            sys.exit()

if __name__ == '__main__':
    main()