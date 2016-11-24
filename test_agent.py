import numpy as np
import ule
from ule.util import jsonparser

def main():
    env = ule.load()
    actions = env.action_space()

    for i in range(500):

        action = actions['Paddle'].sample()
        observation, reward, done, info = env.step(action)

        print(observation['BallSensor'])

        #plt.imshow(observation['image'])
        #plt.show()        

    env.close()

if __name__ == '__main__':
    main()