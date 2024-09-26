import os
import sys

import numpy as np
import pandas as pd

from pathlib import Path
path = Path(__file__).parent.resolve()

class Level():
    
    def __init__(self, shape:tuple = (5, 10)):
        self.shape = shape
        self.layout = [["-" for x in range(shape[0])] for y in range(shape[1])] # 2D array containing the level
        self.background = [["-" for x in range(shape[0])] for y in range(shape[1])] # OPTIONAL: 2d array containing the background to the level
        self.surrounding = {}

        print(self.layout)
        pass

    def load(self, level_f:str):
        # Load in a level from a level file

        is_keys = False # check to see if looking at surrounding levels
        is_room = False # check to see if looking at level

        level_contents = open(level_f, 'r').readlines()
        for i, line in enumerate(level_contents):
            contents = line.split(' ')
            # Read through level file line by line and construct a Level Object from it
            if len(line) == 0: # case: emtpy line
                continue
            elif i == 0: # case: first line contains the shape of the level
                self.shape = (int(contents[0]), int(contents[1]))
                print(f"Creating new level of size {self.shape}")

                # build empty level
                try:
                    self.layout = [["-" for x in range(self.shape[0])] for y in range(self.shape[1])]
                except Exception as e:
                    print(f"Error: {e} -> load()")
                    return
                pass
            elif "KEY" in line: # case: surrounding levels
                is_keys = True
                pass
            elif is_keys:
                if len(contents) == 0: # finished checking keys
                    is_keys = False
                    continue
                elif len(contents) == 1: # empty key
                    continue
                self.surrounding[contents[0]] = contents[1]
                pass
            elif "ROOM" in line:
                is_room = True
            elif is_room:
                if len(contents) == 0: # finished checking room
                    is_room = False
                    continue
                elif len(contents) != self.shape[1]: # error with length of room
                    raise Exception('Error: Incorrect level shape --> load()')
        pass
    
    def save(self, level_f:str):
        # save this level to <level_f> file
        # see below for an example level file

        level_file = open(level_f, 'w')

        level_file.write(f'SHAPE {self.shape[0]},{self.shape[1]}\n\n')
        
        level_file.write(f'INSIZE ')
        level_file.write(f'OUTSIZE ')

        level_file.write(f'INPOS ')
        level_file.write(f'OUTPOS ')

        level_file.write(f'ROOM\n')
        for i in range(self.shape[0]):
            level_file.write(f'{' '.join(self.layout[i])}')

        pass

    def print(self):
        print(f"=====================================")
        print(f"-- Printing level of size {self.shape} --")
        print(f"=====================================")
        row_str = ""
        for i, row in enumerate(self.layout):
            for val in row:
                row_str += val + " "
            print(f"{(i+1):02}: {row_str}")
            row_str = ""
        pass

if __name__ == "__main__":
    # DEBUG: Sample Usage
    test_lvl = Level()
    test_lvl.load(f"{path}/../PCGLevels/Rooms/1.txt")
    test_lvl.print()

    sys.exit()