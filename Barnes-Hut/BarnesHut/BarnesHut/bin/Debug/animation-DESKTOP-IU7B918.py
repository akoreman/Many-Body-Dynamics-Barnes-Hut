# -*- coding: utf-8 -*-
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.animation 
from matplotlib.animation import FuncAnimation


import InputOutput as IO

array = IO.ReadCSV("animation.csv")

xarray = array[0::3]
yarray = array[1::3]
zarray = array[2::3]


a = 5

fig, ax = plt.subplots()  
ax.set(xlim=(-a, a), ylim=(-a, a))
ax.grid()  

#data = np.cumsum(np.random.normal(size=100)) #some list of data
sc, = ax.plot(xarray[0], yarray[0], marker="o", ls="", markersize = 2) # set linestyle to none

def plot(i):
    sc.set_data(xarray[i], yarray[i])
    #print(xarray[i])

ani = matplotlib.animation.FuncAnimation(fig, plot, 
            frames=len(xarray) - 1, interval= 1, repeat=False) 
 
plt.show()

#print(xarray[3])
