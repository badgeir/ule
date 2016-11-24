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
    def __init__(self, low, high, length, name=''):
        """
        Two kinds of valid input:
            Box(-1.0, 1.0, (3,4)) # low and high are scalars, and shape is provided
            Box(np.array([-1.0,-2.0]), np.array([2.0,4.0])) # low and high are arrays of the same shape
        """
        assert np.isscalar(low) and np.isscalar(high)
        self.low = low + np.zeros(length)
        self.high = high + np.zeros(length)
        self.name = name

    def sample(self):
        return prng.np_random.uniform(low=self.low, high=self.high, size=self.low.shape)
    def contains(self, x):
        return x.shape == self.shape and (x >= self.low).all() and (x <= self.high).all()

    def to_jsonable(self, sample_n):
        return np.array(sample_n).tolist()
    def from_jsonable(self, sample_n):
        return [np.asarray(sample) for sample in sample_n]

    def name(self):
        return self.name

    @property
    def length(self):
        return self.low.size
    def __repr__(self):
        return "Vector" + str(self.length)
    def __eq__(self, other):
        return np.allclose(self.low, other.low) and np.allclose(self.high, other.high)