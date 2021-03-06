# kOS-EVA

## What is it?
This is a collection of KSP&kOS addons which provide the ability of EVAed Kerbals to be controlled by kOS scripts.

## Requirements

* kOS
* (KIS) only for the kOS-Pad

## How to install

Unpack the kOS-EVA.zip file in the GameData folder

if you want to use the Brain Implants: unzip the kOS-EVA-4all.zip in the GameData folder.

## How to Use
If you use the Kerbal-Brain-Implant (KBI): Go EVA with a Kerbal.

Or the kOS-Pad as a KIS part to the seat-place of the kerbals you want to control. When they go EVA they automatic equip the pad.

All Kerbals boot the file "/boot/eva". You should add some communication with the ship, to differentiate the kerbal tasks.

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

**:TOGGLE_RCS(\<bool\>)**

Switches the RCS of a floating Kerbal on or off. 

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


**:PLANTFLAG(\<FlageSiteName\>,\<Plaque Text as a string\>)** 

Plants a Flag. The flag site will be named \<FlageSiteName\> and has the plaque field set.


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


## Changelog
### 0.1
* Ported the addon to KSP 1.2.
* Created ModuleManager .cfg files for the Kerbal Modules

### 0.0.93
* Kerbal Brain Implants now working. Every Kerbal is now a walking kOS Computer. 

### 0.0.92
* Bugfix: newly spawned kerbals are now controllable again.

### 0.0.91 
* enabled Actiongroups 
* changed :plantflag() to :plantflag("sitename","Plaque Text").

### 0.0.9
* Initial Release

## future Plans

* integrate Brain Implants into Techtree.
* better/real RCS movement in space. 
* KIS support
* better animation changes.
* add a second Parameter to :GOEVA to define a bootfile for newly EVAed Kerbals.



## Known Bugs and limitations

* automatic boot does not always work with RemoteTech.
* limited control in Space with RCS thrusters.
* the animations are not smoothed out, when there is a change in animations.

