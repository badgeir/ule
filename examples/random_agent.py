
import ule


def main():

    unity_project_path = "C:\\DeepLearning\\reinforcement\\ule\\unityproj"
    exe_name = "pong"
    env = ule.Env(unity_project_path=unity_project_path, name=exe_name, port=3001)

    while True:
        done = 0
        total_reward = 0
        while done == 0:
            for motor in env.motors:
                motor.randomize()

            reward, done, info = env.step()

            total_reward += reward

        print('game over, total reward: %f\n'%(total_reward))
        
        env.reset()

    env.close()

if __name__ == '__main__':
    main()