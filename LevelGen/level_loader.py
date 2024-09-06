import os
import sys

# import numpy as np
# import pandas as pd

from level import Level


# DUPLICATE FUNCTION CAN REMOVE
def load_level_from_txt(in_file:str) -> Level:

    out_level = Level()
    is_level = False

    with open(out_level, 'r') as level_f:
        for line in level_f.readlines():
            print(line) # DEBUG
            if("SHAPE" in line):
                out_level.shape = (line.split(' ')[1], line.split(' ')[2])
            elif("ROOM" in line):
                is_level = True
            elif("INPOS" in line):

            elif(is_level):
                if( len(contents) == 0):
                    is_level = False
                    continue
                elif len(contents != out_level.shape[1]:
                    raise Exception('Error')
                out_level.layout.append([x for x in line.split(' ')])
            
        pass
    
    return out_level

if __name__ == "__main__":
    # DEBUG: Sample Usage
    
    sys.exit()