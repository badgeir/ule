import numpy as np
import ule

def main():
    env = ule.load()
    sensors = env.sensors()
    motors = env.motors()

    for game in range(5):
        done = 0
        total_reward = 0
        while done == 0:
            for motor in motors:
                motor.randomize()

            reward, done, info = env.step()
            
            total_reward += reward

        print('game %d, total reward: %f\n'%(game, total_reward))
        
        env.reset()

    env.close()

if __name__ == '__main__':
    main()