import os
import sys
import random

import matplotlib.pyplot as plt

from level import Level
from perlin_noise import PerlinNoise


# File containing various algorithms to manipulate existing levels + generate levels

def perlin_noise_gen_background(lev: Level=None):
    # manipulates a level using perlin noise

    if lev is None:
        print("error: level object is empty")
        return

    # DEBUG: display perlin noise graph
    # pic = [[perlin_noise([i/xpix, j/ypix]) for j in range(xpix)] for i in range(ypix)]
    # plt.imshow(pic, cmap='gray')
    # plt.show()

    # generate perling noise square
    perlin_noise = PerlinNoise(octaves=10)
    xpix, ypix = lev.shape[0], lev.shape[1]
    perlin_arr = [[perlin_noise([i/xpix, j/ypix]) for j in range(xpix)] for i in range(ypix)]
    
    # DEBUG: show perlin map
    plt.imshow(perlin_arr, cmap='gray')
    plt.show()

    for j in range(lev.shape[0]):
        for i in range(lev.shape[1]):
            # print(f"editing block at ({x}, {y})")
            # lev.background[x][y] = "B" if perlin_arr[x][y] > 0.5 else lev.background[x][y] = "-"
            try:
                if(perlin_arr[i][j] < -0.1): lev.background[i][j] = "B"
                elif(perlin_arr[i][j] < -0.05): lev.background[i][j] = "b"
                else: lev.background[i][j] = "-"
            except:
                print(f"error: {i},{j} is out of range of background arr")
    
    lev.print()

    pass

if __name__ == "__main__":
    level = Level()
    level.load("../Levels/Parts/Middle/ME1_10.txt")
    perlin_noise_gen_background(level)
    sys.exit()