# Planetary Dynamics using Barnes-Hut tree algrotihm
Program to simulate planetary dynamics using the Barnes-Hut tree algorithm and
the brute force method. Main goal of the project was to produce animations and to show
the speedup offered by the BH method over the brute force method from N^2 to N log N. 

The Barnes-Hut tree algorithm approximates planetary dynamics by building an oct-tree containing the planetary bodies. If a group of bodies is far away
the center-of-mass of a node of the tree is used to approximate the gravitational attraction of a group of bodies.  

See the ppt presentation in root for more details. Barnes-Hut algorithm implemented in C#, animations made using MatPlotLib in Python.

<img src="https://raw.github.com/akoreman/Planetary-Dynamics-Barnes-Hut/main/images/Collision.PNG" width="400"> 

**Time complexity scaling**  
<img src="https://raw.github.com/akoreman/Planetary-Dynamics-Barnes-Hut/main/images/TimeComplexity.PNG" width="400">  
