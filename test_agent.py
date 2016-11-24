import numpy as np
import ule
from ule.util import jsonparser

def main():
    env = ule.load()
    print(env.observation_space().sample())
    print(env.action_space().sample())
    
    env.close()

if __name__ == '__main__':
    main()