import os
import sys

import numpy as np
import pandas as pd

class Level():
    
    def __init__(self, shape:tuple = (10, 10)):
        self.shape = shape
        self.layout = [[]]
        pass

    def load_level(self, level_f:str) -> Level:
        # Load in a level from a level file
        level_contents = open(level_f, 'r').readlines()
        for line in level_contents:
            # Read through level file line by line and construct a Level Object from it
            # print(line) # DEBUG
            if(len(line) == 0):
                continue
            line_arr = line.split(' ')
            print(line_arr[0])
            pass
        return self.Level
    
    def save_level(self, level_f:str):
        # save this level to <level_f> file
        
        pass


if __name__ == "__main__":
    # DEBUG: Sample Usage
    l = Level().load_level("../PCGLevels/1.txt")
    sys.exit()