# -*- coding: utf-8 -*-
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.animation 
from matplotlib.animation import FuncAnimation


import InputOutput as IO

def f(x, a, b):
    return x * np.log10(x)

def g(x, a, b):
    return (a + x)^2 + b

brute = IO.ReadCSV("brute.csv")
barnes = IO.ReadCSV("barnes.csv")

nlist = [x[0] for x in barnes[0::2]]
barnes = [x[0] for x in barnes[1::2]]
brute =[x[0] for x in brute[1::2]]

#coefBrute = np.polyfit(nlist,brute,2)
#coefBarnes = np.polyfit(nlist * np.log(nlist)/np.log(8), barnes, 1)

#fitBrute = np.poly1d(coefBrute)
#fitBarnes = np.poly1d(coefBarnes)

a = len(nlist)

#plt.plot(nlist, fitBrute(nlist))
#plt.plot(nlist, fitBarnes(nlist))
plt.loglog(nlist[:a], barnes[:a], 'bo', markersize = 5)
plt.loglog(nlist[:a], brute[:a], 'ro', markersize = 5)
plt.xlabel("Number of Bodies N",fontsize=18)
plt.xticks(fontsize = 14)
plt.ylabel("Runtime per step (milliseconds)",fontsize=18)
plt.yticks(fontsize = 14)
plt.tight_layout()
plt.legend(['Barnes-Hut', 'Brute Force'], loc='upper left', fontsize = 14)
plt.savefig("complexity.pdf")
plt.show()

np.log