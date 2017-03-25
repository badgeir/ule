# modified version of openai gyms Box (MIT License)
# Copyright (c) 2016 OpenAI (http://openai.com)

import numpy as np
from ule.spaces import prng
from ule.spaces.space import Space

import io
from matplotlib import image as mpimg

import base64


class Image(Space):
    """
    A box in R^n.
    I.e., each coordinate is bounded.
    Example usage:
    self.action_space = spaces.Box(low=-10, high=10, shape=(1,))
    """
    def __init__(self, low, high, shape=None):
        """
        Two kinds of valid input:
            Box(-1.0, 1.0, (3,4)) # low and high are scalars, and shape is provided
            Box(np.array([-1.0,-2.0]), np.array([2.0,4.0])) # low and high are arrays of the same shape
        """
        if shape is None:
            assert low.shape == high.shape
            self.__low = low
            self.__high = high
        else:
            assert np.isscalar(low) and np.isscalar(high)
            self.__low = low + np.zeros(shape)
            self.__high = high + np.zeros(shape)
    
    def zeros(self):
        return np.zeros(self.shape)

    def sample(self):
        return prng.np_random.uniform(low=self.__low, high=self.__high, size=self.__low.shape)

    def contains(self, x):
        return x.shape == self.shape and (x >= self.__low).all() and (x <= self.__high).all()

    def to_jsonable(self, sample_n):
        return np.array(sample_n).tolist()

    def from_jsonable(self, sample_n):
        imgbytes = base64.b64decode(sample_n)
        img = None
        try:
            img = mpimg.imread(io.BytesIO(imgbytes))
            return img
        except Exception as e:
            print('not an image')

    @property
    def shape(self):
        return self.__low.shape

    def __repr__(self):
        return "Image" + str(self.shape)

    def __eq__(self, other):
        return np.allclose(self.__low, other.low) and np.allclose(self.__high, other.high)