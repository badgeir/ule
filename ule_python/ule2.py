import sys

#add include folders
sys.path.insert(0, 'env')
import ule_env

def load(name='none', connectToRunning=True):
    if not connectToRunning:
        if ule_env.start(name):
            print('successfully started environment %s.'%name)
    
    env = ule_env.Env()
    env.connect()
    return env