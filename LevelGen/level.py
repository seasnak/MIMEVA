import os
import sys

import numpy as np
import pandas as pd

from enum import Enum

def block_type(Enum):
    
    pass

class Level():
    
    def __init__(self, shape:tuple = (10, 10)):
        self.shape = shape
        self.layout = [[]]
        pass

    def load_level(self, level_f:str):
        level_contents = open(level_f, 'r').readlines()
        for line in level_contents:
            # Read through level file line by line and construct a Level Object from it
            # print(line) # DEBUG
            if(line[0] == )
            pass
        pass


if __name__ == "__main__":
    # DEBUG: Sample Usage
    l = Level().load_level("../PCGLevels/1.txt")
    sys.exit()