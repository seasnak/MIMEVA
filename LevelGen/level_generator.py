import os
import sys

import random
import perlin_numpy

from level import Level

# File containing various algorithms to manipulate existing levels + generate levels

def perlin_noise_level_manip(lev: Level):
    # manipulates a level using perlin noise
    perlin_noise = perlin_numpy.generate_perlin_noise_2d((10, 10), (1, 1), (False, False))    
    
    pass

if __name__ == "__main__":

    sys.exit()