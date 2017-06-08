import ule
from baselines import deepq


def main():
    unity_project_path = r"D:\Web\ule\ule\unityproj"
    exe_name = "pong"
    env = ule.Env(unity_project_path=unity_project_path, name=exe_name, port=3001)
    gymenv = ule.GymWrapper(env)
    model = deepq.models.cnn_to_mlp(
        convs=[(32, 8, 4), (64, 4, 2), (64, 3, 1)],
        hiddens=[256],
        dueling=True
    )
    act = deepq.learn(
        gymenv,
        q_func=model,
        lr=1e-4,
        max_timesteps=2000000,
        buffer_size=10000,
        exploration_fraction=0.1,
        exploration_final_eps=0.01,
        train_freq=4,
        learning_starts=10000,
        target_network_update_freq=1000,
        gamma=0.99,
        prioritized_replay=True
    )
    act.save("pong_model.pkl")
    env.close()


if __name__ == '__main__':
    main()
