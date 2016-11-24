import numpy as np
import ule

def main():
    env = ule.load()
    
    for i in range(500):
        observation, reward, status = env.step(np.random.randint(3))
        print(observation['BallSensor'])
    
    env.close()

if __name__ == '__main__':
    main()