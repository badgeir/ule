from ule.spaces import Vector
import json

class Sensor(object):

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
        self._value = self._space.from_json(jsonstr)

    def __repr__(self):
        return self._space.__repr__() + ' Sensor: %s'%str(self._value)
    def __str__(self):
        return self._space.__repr__() + ' Sensor: %s'%str(self._value)