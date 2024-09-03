import os
import sys

# import numpy as np
# import pandas as pd

from level import Level

def load_level_from_txt(in_file:str) -> Level:

    out_level = Level()

    with open(out_level, 'r') as level_f:
        for line in level_f.readlines():
            print(line) # DEBUG
            if("SHAPE" in line):
                out_level.shape = out_level.shape
        pass
    
    return out_level

if __name__ == "__main__":
    # DEBUG: Sample Usage
    
    sys.exit()