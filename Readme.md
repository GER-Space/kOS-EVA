# kOS-EVA

## What is it?
This is a collection of KSP&kOS addons which provide the ability of EVAed Kerbals to be controlled by kOS scripts.

## Requirements

* kOS
* KIS

## How to install

Unpack the .zip file in the GameData folder

## How to Use
add the kOS-Pad as a KIS part to the seat-place of the kerbals you want to control. When they go EVA they automatic equip the pad.
The Pad tries to boot the file "/boot/eva". 

## kOS Addon Functions:
 
Basepath is **addons:eva**  
 
### From a Ship:

**:GOEVA(\<crewmember\>)**  
Kicks the unlucky kerbal out of his nice vessel. (ex: addons:eva:goeva(SHIP:CREW[0]).

### On EVA

**:DOEVENT(\<part\>,\<eventname\>)**  

Executes the event on the part which starts with the string \<eventname\>. It only works when the kerbal is closer than 1.5m to the part. 
*example:* 
>set part to vessel("pad test"):parts[0].  

>addons:eva:doevent(part,"Store Experiments").
 
**:LADDER_GRAB**

Grab a nearby ladder



**:LADDER_RELEASE**

Release a grabbed ladder



**:TURN_LEFT(\<degrees\>)** 

Make the kerbal turn left by \<deg\>.



**:TURN_RIGHT(\<degrees\>)**

Make the kerbal turn right by \<deg\>.
 
 
 
**:TURN_TO(\<position_vector\>)** 

Make the kerbal turn to a \<vector\>.



**:MOVE(\<what\>)**

The Kerbal will move in that direction.

#### on land ####
* Forward
* Backward
* Left
* Right
* Stop


#### on a Ladder ####
* Up
* Down
* Stop


#### in Water ####
* Forward,
* Stop.


**:BOARDPART(\<Part\>)** 

Enters the Part


**:BOARD**

Board a nearby vessel or part. The normal KSP rules of boarding apply.


**:PLANTFLAG** 

Plants a Flag. This requires in the moment a confirmation with the mouse.


**:ACTIONLIST** 

List of all things a Kerbal can do.


**:RUNACTION(\<Actionname\>)** 

Runs a Kerbal action by its name (ex.: addons:eva:runaction("Jump Start") )


**:ANIMATIONLIST**

List of all animation names.


**:LOADANIMATION(\<path\>)**

Loads a custom animation by its relative pathname into the list of animations. The custom animations are created by [KerbalAnimationSuite](http://forum.kerbalspaceprogram.com/index.php?/topic/117663-113-kerbal-animation-suite/ ) 

example: 

> addons:eva:LOADANIMATION("\kOS-Pad\Anims\Wave.anim").


**:PLAYANIMATION(\\<name\\>)** 

Runs a animation by its \<name\> shown by *:animationlist*.

> addons:eva:PLAYANIMATION("Wave").


**:STOPANIMATION(\<name\>)** 

Stops the Animation named \<name\>.

 
**:STOPALLANIMATIONS"** 

Stops all Animations and returns the kerbal to his idle state.




## future Plans

* movement in space
* automatic planting of a flag
* KIS support
* reenable actionsgroups
* better animation changes.
* Kerbal brain implants (making this addon work without KIS)



## Known Bugs and limitations

* automatic boot does not work with RemoteTech enabled. (no easy workaround in the queue)
* No control in Space or when flying
* the animations are not smoothed out, when there is a change in animations.
* the default actionsgroups keys are not implemented and worse: they are overridden by the default KIS configuration.
* planting a flag requires a confirmation. 

