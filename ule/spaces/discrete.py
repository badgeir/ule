# modified version of openai gyms Discrete (MIT License)
# Copyright (c) 2016 OpenAI (http://openai.com)

import numpy as np
from ule.spaces import prng
from ule.spaces.space import Space


class Discrete(Space):
    """
    {0,1,...,n-1}
    Example usage:
    self.observation_space = spaces.Discrete(2)
    """
    def __init__(self, n):
        self.n = n

    def zeros(self):
        return 0

    def sample(self):
        return prng.np_random.randint(self.n)

    def contains(self, x):
        if isinstance(x, int):
            as_int = x
        elif isinstance(x, (np.generic, np.ndarray)) and (x.dtype.kind in np.typecodes['AllInteger'] and x.shape == ()):
            as_int = int(x)
        else:
            return False
        return 0 <= as_int < self.n

    def to_jsonable(self, sample_n):
        return int(sample_n)

    def from_jsonable(self, sample_n):
        pass

    def __repr__(self):
        return "Discrete(%d)" % self.n

    def __eq__(self, other):
        return self.n == other.n