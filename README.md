# 2D Collision Simulation
A unity project that simulates 2D particle (circles) moving around and colliding with each other using neuton's laws.
For Optimization im using bounding volume hierarchy method,
Construction of a BVH typically takes O(n log n) time, while traversal for intersection tests is O(log n) in balanced scenarios, though O(n) in worst-case. 

5k particles runing at a stable 60fps:
![5k particles](https://github.com/user-attachments/assets/51f555a1-937f-4e93-b137-f27826d9df3f)

playing around in the editor:
![show off](https://github.com/user-attachments/assets/267b8a49-8361-4c1c-ac42-3e5894816b23)

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
The project is finished, it only needs some finish touches that i dont really care at the moment to work on.
