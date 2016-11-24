from ule.spaces import Vector, Discrete
import json

class Motor(object):

    def __init__(self, jsonlist):
        self._name = None
        self._space = None
        self.create_from_json(jsonlist)
        self._value = self._space.zeros()

    def name(self):
        return self._name
    
    def value(self):
        return self._value
    
    def sample(self):
        return self_space.sample()

    def space(self):
        return self._space

    def create_from_json(self, jsonlist):
        self._name = str(jsonlist['name'])
        if jsonlist['type'] == 'vector':
            min = float(jsonlist['min'])
            max = float(jsonlist['max'])
            size = int(jsonlist['size'])
            self._space = Vector(min, max, size)
        elif jsonlist['type'] == 'discrete':
            n = int(jsonlist['range'])
            self._space = Discrete(n)
        else: 
            raise Exception('Unknown sensor type')
        
    def parse_to_json(self):
        pass