from ule.spaces import Vector
import json

# Space-related abstractions
class Sensor(object):
    """Defines the observation and action spaces, so you can write generic
    code that applies to any Env. For example, you can choose a random
    action.
    """
    def __init__(self, jsonlist):
        self._name = None
        self._space = None
        self.create_from_json(jsonlist)
        self._value = self._space.zeros()


    def name(self):
        return self._name
    
    def value(self):
        return self._value
    
    def space(self):
        return self._space

    def create_from_json(self, jsonlist):
        self._name = str(jsonlist['name'])
        if jsonlist['type'] == 'vector':
            min = float(jsonlist['min'])
            max = float(jsonlist['max'])
            size = int(jsonlist['size'])
            self._space = Vector(min, max, size)
        else: 
            raise Exception('Unknown sensor type')
        
    def value_from_json(self, jsonstr):
        pass