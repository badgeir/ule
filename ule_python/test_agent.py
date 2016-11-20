import numpy as np
import time
import sys

from matplotlib import pyplot as plt
import ule

def main():
    env = ule.ULEIface()

    starttime = time.clock()
    while True:
        try:
            img, reward, status = env.step(0)

            print (img.shape)
            print(img.dtype)
            print(reward)
            print(status)

            dtime = time.clock() - starttime
            print(dtime)
            starttime = time.clock()

        except Exception as e:
            sys.exit()

if __name__ == '__main__':
    main()