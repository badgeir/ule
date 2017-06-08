

class GymWrapper(object):
    def __init__(self, env):
        self.env = env

    def step(self, action):
        self.env.motors[0].set_value(action)
        rew, done, info = self.env.step()
        obs = self.env.sensors[0].value()

        return obs, rew, done, info

    @property
    def action_space(self):
        return self.env.motors[0].space()

    @property
    def observation_space(self):
        return self.env.sensors[0].space()

    def reset(self):
        self.env.reset()
        return self.env.sensors[0].value()
