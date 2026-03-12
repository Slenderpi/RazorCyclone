# _Razor Cyclone_ - Shoot to Move!
## What is _Razor Cyclone?_
_Razor Cyclone_ is a single-player, **first-person endless-arena movement shooter**. The game’s core mechanic revolves around rotating the weapon and **shooting to move**, requiring players to eliminate enemies to replenish their ammo.

The player has **2 weapons**, each one having its own **unique movement and attack patterns**. The player must rotate their weapon (which is a bike) and shoot in various directions to move around and attack enemies.

## Play the game!
You can download and play _Razor Cyclone_ from itch.io: https://expiredjam.itch.io/razorcyclone

## What is this github page?
This repository contains the project files of _Razor Cyclone._ You can take a peek at how this game works!

## What is the development status of _Razor Cyclone?_
The main game's project files are on the `main` branch. On this version, all gameplay features are fully implemented and playable. However, there is some unfinished work in game balancing, and the Almanac (a sort of 'wiki' players can look at for more details about gameplay mechanics) is missing content. Right now, this branch is inactive.

Currently, the only active branch is the [`DOTS`](https://github.com/Slenderpi/RazorCyclone/tree/DOTS) branch, an experimental branch intended for massively optimizing _Razor Cyclone_.

## The development team
_Razor Cyclone_ is developed by [Expired Jam](https://expiredjam.itch.io/), a team of students that came together at Purdue University. \
You can talk with us and other players of _Razor Cyclone_ in our discord: https://discord.gg/B5fk4WnH

Game Design and Direction - [Adam Malas](https://www.adammalas.com/) \
Production - [Libby Tucker](https://www.tuckerelizabeth.com/) \
Programming and VFX - [Preston Fu](https://www.prestonfu314.com/) \
Art lead and Character art - [Evan Vassar](https://evanvasser.wixsite.com/evan-vasser) \
Enviromental art - [Luke Boken](https://aspenaspires.wixsite.com/lukeboken) \
Sound and Music - [Owen Fischer](https://www.linkedin.com/in/owen-fischer/)

---

# DOTS x _Razor Cyclone_ - Boosting framerates by over 1000%
In this branch, I will utilize Unity's [Data-Oriented Technology Stack](https://unity.com/dots), or DOTS, to optimize many features of *Razor Cyclone*. \
The current version of *Razor Cyclone* has performance issues in some areas. This is more noticeable in later waves of the game where the enemy count becomes quite high.

Unfortunately, a feature implemented using GameObjects is not immediately compatible with DOTS, both in terms of the technology and also the fundamental principles regarding implementation (OOP vs DOD). This means that I will have to ***recreate over 90% of the game.***

This work is also a sort of research project for me, where I learn more about and dive deeper into things like data-oriented design, multi-threading, C#, and the Unity engine. \
Videos of some of the progress can be seen [on my twitter/x page](https://x.com/slenderpi_senpi).

### What's wrong with the main version of *Razor Cyclone*? Why not optimize that version instead?
I want to explore some new stuff. DOTS and data-oriented design principles provide interesting methods towards game optimization. Early on in this experimental branch, I created a simple test to see if the performance benefits of DOTS were strong enough to be worth it. The result: DOTS is *very* fast!

However, as stated earlier, usage of DOTS would require me to redo almost the entirety of the code in *Razor Cyclone.* Might as well start anew.

Furthermore, *Razor Cyclone* has a lot of messy code (e.g. overly-coupled classes, poor implementation of the Centipede (though it's more poor in the sense that Heap Sort is theoretically fast but practically slow), mistakes regarding usage of Unity's UI). Restarting is a simple way of fixing old code.

## Features
### Pathfinding
In the main branch, enemies do not have pathfinding. They instead rely on Boid Wander (random movement) and the level's layout to hopefully not get stuck on walls too often. However, they still occasionally do get stuck, and that is neither good looking nor fun to play against.

One of the first features implemented in this DOTS branch was a simple method for pathfinding. It is voxel-based and uses BFS. To understand these choices, the circumstances of the game should be discussed:
1. **Enemies can fly.** Some enemies, like the Hunter, move by flying. Traditional Navmeshes do not provide features sufficient for interesting flying movements. A method for pathfinding in three dimensions is necessary. Voxels (3D cubes of space) are good for this.
2. **There will be many, many enemies.** The purpose of this branch is to heavily optimize the game. An optimized game can support more enemies running at once. A simple, all-to-one pathfinding system can be implemented to provide pathfinding capabilities to all enemies, regardless of how many there are. BFS is a simple and effective algorithm for this.
3. **The level is static.** I can bake a voxel-representation of the map during development, then load it at runtime.
As a result, I have created components, systems, and a class to implement a voxel-based BFS pathfinding system. Things that have to do with the voxel representation of the level will have "PointCloud" (or just "Point) in their names (at the time of implementation, I did not think about the term 'voxels', so I went with 'PointCloud' instead). Things more specific to movement will have the term "Wavefront" in their name (relating to the Wavefront Algorithm, which is just BFS).

The pathfinding roughly works as follows:
1. During development, I can run my PointCloudGenerator to bake a level and get a voxel representation of the level, which I've internally called a "PointCloud". During runtime, the baked PointCloud is loaded and used.
2. Every X seconds (configurable in development), BFS is run on the PointCloud, producing a new array (or rather DynamicBuffer, since all of this has to be compatible with DOTS) of vectors. Each vector points to a neighboring Point in the PointCloud, that, if continuously followed, would eventually lead to the Player.
3. Every frame, any Entity with the WavefrontReader component will be told what wavefront vector it should follow to reach the Player, based on their position in the PointCloud. A system specific to that Entity can then do whatever they need to do with their WavefrontReader's value (e.g. follow it to reach the player, or go in the opposite direction to flee).

### Optimized Enemies
All enemies will be optimized to work with DOTS, the Burst compiler, and be multithreaded.

## Experimental experimental branches
There will be some branches that are derived from the main DOTS branch. Such branches will be named "DOTS-SomeIdea". These branches are ideas I have for further optimization in specific areas of the game, but am unsure of if it's worthwhile or how to best approach it. Such branches may never be completed and only left as an idea.

Here is a list of the current experimental experimental branches:
- [DOTS-SparseVoxelOctree](https://github.com/Slenderpi/RazorCyclone/tree/DOTS-SparseVoxelOctree): SVOs would be more efficent than the current constant-size voxels in the PointCloud in terms of both time and memory. However, I am unsure of how to use it in DOTS. Additionally, the current PointCloud method works well enough.
- [DOTS-PointCloudRaycast](https://github.com/Slenderpi/RazorCyclone/tree/DOTS-PointCloudRaycast): a method of performing a raycast using the PointCloud. A basic implementation proves that it is faster than using Unity's physics-based raycasting, but the performance gain may not be worth enough to warrant further work on it. I might come back to it if raycasts become a bottleneck.
