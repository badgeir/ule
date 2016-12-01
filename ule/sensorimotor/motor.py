from ule.spaces import Vector, Discrete


class Motor(object):

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
    
    def set_value(self, value):
        self._value = value

    def randomize(self):
        self._value = self._space.sample()

    def contains(self, sample):
        return self._space.contains(sample)

    def create_from_json(self, jsonlist):
        self._name = str(jsonlist['name'])
        if jsonlist['type'] == 'vector':
            minval = float(jsonlist['min'])
            maxval = float(jsonlist['max'])
            size = int(jsonlist['size'])
            self._space = Vector(minval, maxval, size)
        elif jsonlist['type'] == 'discrete':
            n = int(jsonlist['range'])
            self._space = Discrete(n)
        else: 
            raise Exception('Unknown sensor type')
        
    def to_jsonable(self):
        jsn = {'name': self._name, 'value': self._space.to_jsonable(self._value)}
        return jsn

    def __repr__(self):
        return self._name + ": " + str(self._space) + ' Motor'

    def __str__(self):
        return self._name + ": " + str(self._space) + ' Motor'

    def __getitem__(self, idx):
        try:
            return self._value[idx]
        except Exception as e:
            print str(self._space) + ' Motor does not support indexing'
    
    def __setitem__(self, idx, value):
        try:
            self._value[idx] = value
        except Exception as e:
            print str(self._space) + ' Motor does not support indexing'
