import os
import sys
import random

import matplotlib.pyplot as plt

from level import Level
from perlin_noise import PerlinNoise


# File containing various algorithms to manipulate existing levels + generate levels

def perlin_noise_gen_background(lev: Level=None):
    # manipulates a level using perlin noise
    
    perlin_noise = PerlinNoise(octaves=10)
    xpix, ypix = 20, 20

    # DEBUG: display perlin noise graph
    # pic = [[perlin_noise([i/xpix, j/ypix]) for j in range(xpix)] for i in range(ypix)]
    # plt.imshow(pic, cmap='gray')
    # plt.show()

    

    pass

if __name__ == "__main__":
    perlin_noise_gen_background()
    sys.exit()