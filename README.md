# Planetary Dynamics using Barnes-Hut tree algrotihm
Program to simulate many-body dynamics using the Barnes-Hut tree algorithm and
the brute force method. Main goal of the project was to produce animations and to show
the speedup offered by the BH method over the brute force method from N^2 to N log N. 

The Barnes-Hut tree algorithm approximates many-body dynamics by building an oct-tree containing the bodies. If a group of bodies is far away
the center-of-mass of a node of the tree is used to approximate the attraction of a group of bodies.  

See the ppt presentation in root for more details. Barnes-Hut algorithm implemented in C#, animations made using MatPlotLib in Python.

### Curently Implemented:
- Initialize an initial collection of bodies by adding groups of bodies according to a number of distributions.
- Calculate forces between those bodies using either brute-force or the BH tree.
- Use a number of integration techniques to cacluate new velocites and positions of the bodies.
- Write these positions frame-by-frame to file.
- Animate those files in animations as can be seen below.

### Images:

<img src="https://raw.github.com/akoreman/Planetary-Dynamics-Barnes-Hut/main/images/Collision.PNG" width="400"> 

<img src="https://raw.github.com/akoreman/Planetary-Dynamics-Barnes-Hut/main/images/CollisionAnim.gif" width="400"> 

**Time complexity scaling**  
<img src="https://raw.github.com/akoreman/Planetary-Dynamics-Barnes-Hut/main/images/TimeComplexity.PNG" width="400">  
