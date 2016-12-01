from ule.spaces import Vector, Image


class Sensor(object):

    def __init__(self, jsonlist):
        self._name = None
        self._space = None
        self.create_from_json(jsonlist)
        self._value = self._space.zeros()

    def name(self):
        return self._name

    def space(self):
        return self._space

    def value(self):
        return self._value

    def create_from_json(self, jsonlist):
        self._name = str(jsonlist['name'])
        if jsonlist['type'] == 'vector':
            minval = float(jsonlist['min'])
            maxval = float(jsonlist['max'])
            size = int(jsonlist['size'])
            self._space = Vector(minval, maxval, size)
        elif jsonlist['type'] == 'camera':
            minval = 0
            maxval = 1
            width = int(jsonlist['width'])
            heigth = int(jsonlist['heigth'])
            dim = int(jsonlist['channels'])
            self._space = Image(minval, maxval, (width, heigth, dim))
        else: 
            raise Exception('Unknown sensor type')
        
    def value_from_json(self, jsonstr):
        self._value = self._space.from_jsonable(jsonstr)

    def __repr__(self):
        return self._name + ": " + str(self._space) + ' Sensor'

    def __str__(self):
        return self._name + ": " + str(self._space) + ' Sensor'
