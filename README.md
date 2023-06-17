## MouseTrap

#Mouse Trap Game Dev Notes

2/16

Intent is to clone game I played on cool math games .com when I was younger
https://www.mathplayground.com/logic_trap_the_mouse.html
A mouse spawns in the middle of a grid of hexagons. The user can select a hexagon on the 
grid and it will raise a barrier. Goal is to stop the mouses escape off the grid

Components that will need programmed:

    Mouse AI
    Map
    User-Click Action
    Win/Lose Scenario
    Menus 

I'm going to actually make some notes this time so that I remember how to do this 
in the future

Opened Unity - See very unforgiving blank screen

In lefthand pane, there is the scene hieracrchy. In here, right-click, add an empty object
    - Made one for the mouse, will give it life later with a sprite and scripting

For the map, need to consider best move...
    Use prefab? Probably. 

    A prefab, IIRC, is like a template gameobject that can be instantiated multiple times 

    Attached a sprite renderer (picture of a hexagon) and a script to the prefab

    Plans:
        - Some outside scripting (on-load) will instantiate many MapHex's in their correct locations 
        - Fields/Parameters:
            - isClicked
            - isMouseOn
            - isEdge?
        - Functions:
            - Clicked

Not sure if this is a "hacky" way to do this, but I made a "main" object in my hierarchy and put the mouse underneath it. 
then I attached a manager.cs script that will be responsible for placing the map hexes and mouse. I just feel like I need a driver 
scirpt that isn't related to either entity

Stole some old code to instantiate the map hexes on start -- remembered that uninitialized variables (like gameobject prefabs) 
can be clicked and dragged into the property editory in unity after they are declared in c#

Sprite edges were kinda fuzzy - played with the sprites some to try to alliviate

Accidentally found an algorithm that generates a hex grid -- stole it and repurposed

Added colliders and hit detection code to the prefab

multiple options for the mouse - 
	* save locstion of each hex center?
	* have mouse sprite separated from mouse AI. sprite could be in the hex obj
	* do it the mathy way and calc next location

Game Ideas:
	* Levels, i.e. 1,2,3...15
	* Early levels have some hexes already black
    * Later levels multiple mice

2/17

Considering starting on Mouse today. 

Spanws at 0,0 hex, and needs to do the following:

    1) determine POSSIBLE moves 
        see above possilbe methods. Another possible method is sending rays at angles and fetching the GO we hit?
        Impossible move == black hex, other mouse(?)
    2) AI best move
        tmeporarily just pick a random move

2/18 

Got a lot done

made function to find pssible moves. added a circle collider around the mouse just big enough to hit all hexes arund it
established the random movement for now while i figure out the AI
established some code to (initially) handle winning and losing

TODO:
    * Change colliders on hex's to circles for better fit      - done
    * Fix bug, Mouse doesn't move sometimes on first click?    - investigate - cannot reproduce quickly
    * Fix bug, Mouse doesn't move sometimes at all?
    * Enable Mouse to go off screen (to win)
    * You Win / You Lose Message w/ Restart Button
    * Start game button                                        - done
    * Mouse AI 
    * Level's / Progression, Two mice two clicks, variable map size
    * animation, mouse rigid body movement, hexes animate up as towers

2/26
heres what items are remaining from above list:
enable mouse to go off screen to win
you win / lose message + retry button
levels / progression
animation, rigid body movement 

additional work items:
add random number of already clicked hexes at start 
make camera scale with number of hexes

Test new gitignore -- should add this and only this

3/8
- Add esc menu 
- Save after level complete, not new level start
- make MainMenu UI larger
- Add level number to the game 
- Add multiple mice level 5 
