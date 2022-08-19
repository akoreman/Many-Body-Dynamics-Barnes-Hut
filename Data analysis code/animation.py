# -*- coding: utf-8 -*-
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.animation as animation
from matplotlib.animation import FuncAnimation
from matplotlib import rcParams

import InputOutput as IO

array = IO.ReadCSV("animationcollision.csv")

xarray = array[0::3]
yarray = array[1::3]
zarray = array[2::3]

rcParams['animation.ffmpeg_path'] = //insert ffmpeg_path.

arrayA = xarray
arrayB = yarray

a = 100

fig, ax = plt.subplots()  
ax.set(xlim=(-a, a), ylim=(-a, a))
ax.set_facecolor('black')
ax.spines['bottom'].set_color('white')
ax.spines['top'].set_color('white') 
ax.spines['right'].set_color('white')
ax.spines['left'].set_color('white')
ax.tick_params(axis='x', colors='white')
ax.tick_params(axis='y', colors='white')
#ax.text(0,0,"yeah")
#ax.grid()  

sc, = ax.plot(arrayA[0], arrayB[0], marker="o", ls="", markersize = 4, color = 'white') # set linestyle to none

def plot(i):
    sc.set_data(arrayA[i], arrayB[i])
    plt.title("frame " + str(i), fontsize = 18)

ani = animation.FuncAnimation(fig, plot, 
            frames=len(xarray) - 1, interval= 10, repeat=False) 
 
plt.show()

plt.plot(arrayA,arrayB)
plt.savefig("fig.pdf")

Writer = animation.writers['ffmpeg']
writer = Writer(fps=60, metadata=dict(artist='Me'), bitrate=1800)

ani.save('adf.mp4', writer=writer)