#borrowed by openai gym

import numpy as np
from ule.spaces import prng
from ule.spaces.space import Space

class Discrete(Space):
    """
    {0,1,...,n-1}
    Example usage:
    self.observation_space = spaces.Discrete(2)
    """
    def __init__(self, n, name = ''):
        self.n = n
        self.name = name
    def sample(self):
        return prng.np_random.randint(self.n)
    def contains(self, x):
        if isinstance(x, int):
            as_int = x
        elif isinstance(x, (np.generic, np.ndarray)) and (x.dtype.kind in np.typecodes['AllInteger'] and x.shape == ()):
            as_int = int(x)
        else:
            return False
        return as_int >= 0 and as_int < self.n
    
    def name(self):
        return self.name
    def __repr__(self):
        return "Discrete(%d)" % self.n
    def __eq__(self, other):
        return self.n == other.n