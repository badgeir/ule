# modified version of openai gyms Box (MIT License)
# Copyright (c) 2016 OpenAI (http://openai.com)

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
        self.__low = low + np.zeros(size)
        self.__high = high + np.zeros(size)
        self.__size = size
    
    def zeros(self):
        return np.zeros(self.__size)

    def sample(self):
        return prng.np_random.uniform(low=self.__low, high=self.__high, size=self.__size)
    
    def contains(self, x):
        return x.size == self.__size and (x >= self.__low).all() and (x <= self.__high).all()

    def to_jsonable(self, sample_n):
        return np.array(sample_n).tolist()

    def from_jsonable(self, sample_n):
        return np.array(sample_n).astype('float')

    @property
    def size(self):
        return self.__size

    def __repr__(self):
        return "Vector" + str(self.__size)

    def __eq__(self, other):
        return np.allclose(self.__low, other.__low) and np.allclose(self.__high, other.__high)
