# 2D Collision Simulation
A unity project that simulates 2D particle (circles) moving around and colliding with each other using neuton's laws.
For Optimization im using bounding volume hierarchy method,
Construction of a BVH typically takes O(n log n) time, while traversal for intersection tests is O(log n) in balanced scenarios, though O(n) in worst-case. 

# How To Use
After opening the project in unity (Unity 2021.3.16f1 version was created with):
* Select "SimulationHandler" in the Hierarchy.
* in the inspector you gonna find a script called "Collision Simulation Handler".
* there you can Control all the variables you need for the simulation.
* Start the Scene.
# How To Open Build Mode
* Open the Repo Folder
* Open File Called: "Collision Simulation"
* Open File Called: "build"
* Open: "Collision Simulation.exe"
* (to close the simulation window: press Alt + F4, or Alt + Enter and then Close window button)

# Note
The prject is finished, but it just needs some UI so you can build it and play with it.
