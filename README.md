# Amorphous #

A procedurally generated maze, this is intended to be maze puzzle type of game, inspired by "Archer Maclean's Mercury".

There is intended to be some form of keys/waypoints that can be used to modify access to the goal of the level, such as finding a specific colour or blend of colour.

The number of waypoints is to be adjustable as is the size of the maze grid, as a means of modifying the maze difficulty. 

The "*Depth First Search*" maze generation algorithm was used in the initial implementation, but was discarded for a couple of reasons:

1. It generated paths across the entire grid 

2. I found it difficult to set waypoints across the grid without creating inordinately complex functions to calculate their placements without locking paths or access to the goal.

3. I was unable to guarantee solvable paths using the Depth First Search approach combined with other game mechanics that I want to implement.


----------

## Game Mechanics ##

The player moves through the maze with a First Person camera perspective searching for the 'ColourBlobs' to unlock the level goal.

Colours will essentially be the "keys" in the game, matching the Players colour to that of the goal.

To unlock the goal, the player will need to find a specific colour or blend of colour by 'collecting' ColourBlobs.

By collecting ColourBlobs the Players colour will change, this could be a swap of colours or a blending of colours, so they can access the goal.


----------
## TODO: ##

* change camera implementation to make use of CineMachine, as this should have more flexibility & be more efficient than the current implementation. It may also address the issue of the player moving without providing input.

* add function to randomize the wall materials from a list.
 

----------
**31/01/2025** - Reconstructing the project due to Unity going into an apparent infinite loop, this time beginning with some of the UI (using UI Toolkit), then connecting the game mechanics to it.

Previously had difficulties integrating the UI with the 'waypoint mechanics'.

This approach also meant that some superfluous code segments were eliminated.


**20/01/2025** - Stripped the project back of scripting to incorporate ScriptableObjects, earlier attempt at ScriptableObjects caused problems.

----------

## -- Known issues -- ##
* ~~Maze will occasionally generate and spawn with the player spawnPosition outside of the rest of the maze path, this could be a completely "disconnected island" or adjacent to a path wall.~~
  * Players transform.position is one of the last things set during the GameManager's ***Start()***

* ~~Occasionally either some or all of the walls do not spawn, this has to do with an array Index out of range.~~

* ~~The player will begin to move without input, this may be caused by calculations of the relative position of the camera to the player.~~
  * This was attributed to a connected joystick (but forgotten about, out of sight out of mind), it would provide some small input as it had a coiled USB cable resting on it.
