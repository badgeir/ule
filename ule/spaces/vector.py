#borrowed by openai

import numpy as np

from ule.spaces import prng
from ule.spaces.space import Space

class Vector(Space):
    """
    A box in R^n.
    I.e., each coordinate is bounded.
    Example usage:
    self.action_space = spaces.Box(low=-10, high=10, shape=(1,))
    """
    def __init__(self, low, high, size):
        """
        Two kinds of valid input:
            Box(-1.0, 1.0, (3,4)) # low and high are scalars, and shape is provided
            Box(np.array([-1.0,-2.0]), np.array([2.0,4.0])) # low and high are arrays of the same shape
        """
        assert np.isscalar(low) and np.isscalar(high) and np.isscalar(size)
        self.low = low + np.zeros(size)
        self.high = high + np.zeros(size)
    
    def zeros(self):
        return np.zeros(self.size)

    def sample(self):
        return prng.np_random.uniform(low=self.low, high=self.high, size=self.low.size)
    
    def contains(self, x):
        return x.size == self.size and (x >= self.low).all() and (x <= self.high).all()

    def to_json(self, sample_n):
        return np.array(sample_n).tolist()
    def from_json(self, sample_n):
        return [np.asarray(sample) for sample in sample_n]

    @property
    def size(self):
        return self.low.size
    def __repr__(self):
        return "Vector" + str(self.size)
    def __eq__(self, other):
        return np.allclose(self.low, other.low) and np.allclose(self.high, other.high)