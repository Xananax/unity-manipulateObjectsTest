# Unity Move Objects Test

Here are a few scripts to move objects around.

Mainly you'll find:

## Draggable.cs

Attach to any object in the scene to make all Rigidbodies draggable (or those on a layer you select).  
Break Force (speed) and Break Torque (fast change of direction) that can break the joint between the mouse and the object are dependant on the object's mass that is set in it's Rigibody.

#### Properties

- `Attach to Center of Mass (bool)`: if true, will attach to the center of the object instead of click coordinates
- `Layer Mask (int)`: Use to restrict to certain layers
- `Do Not Break (bool)`: Do not break link between mouse and object, even if strong forces are applied
- `Draw Line (bool)`: if true, will draw a line between the object and the mouse
- `Spring (float)`: How springy the joint is
- `Damper (float)`: How much dampness does the joint uses
- `Distance (float)`: The distance between the mouse and the clicked point
- `Smooth (float)`: How smooth the movement is. 1 is very smooth, 10 follows the mouse almost exactly
- `Break Force Modifier (float)`: gets added to the automatically calculated Break Force. Set it to minus to make the Joint stronger
- `Break Torque Modifier (float)`: gets added to the automatically calculated Break Torque. Set it to minus to make the joint stronger
- `Strength (float)`: How strong the mouse is. The higher this value, the easier it will be to lift heavy objects
- `Line Width (float)`: The width of the line between the object and the mouse
- `Line Material (Material)`: The material of the line (it is advised to set it to something self-illuminated and without shadows; a suitable shader is provided in the repo)
- `Restrict`: allows to restrict movement to the X, Y, or Z plane
	- `x (bool)`: restricts to x axis
	- `y (bool)`: restricts to y axis
	- `z (bool)`: ...
- `Max Distance (float)`: if user drags over that distance, the bond will break
- `Mouse Horizon (float)`: Where does the ray cast from the mouse stops detecting objects

Note: I'm not sure that having the break force and torque automatically calculated from the mass is a good idea; However, I didn't want to add this script on each relevant object and so I had to devise a way to obtain those values automatically. I've added the modifiers and strength in order to keep some control over this value. If you have a better idea, I'm all ears.

-----
## ForceDraggable.cs

Same as Draggable.cs, but uses gravity and forces. In other words, the object you're transporting will still be subject to all the forces that may be applied on it.  
It has all the exact same properties as Draggable.cs


-----
## SimpleDraggable.cs

A simpler draggable class that does not require a rigidbody (but works if a rigidbody is present too).

#### Properties

- `Layer Mask (int)`: Use to restrict to certain layers
- `Draw Line (bool)`: if true, will draw a line between the object and the mouse
- `Smooth (float)`: How smooth the movement is. 1 is very smooth, 10 follows the mouse almost exactly
- `Strength (float)`: How strong the mouse is. The higher this value, the easier it will be to lift heavy objects
- `Line Width (float)`: The width of the line between the object and the mouse
- `Line Material (Material)`: The material of the line (it is advised to set it to something self-illuminated and without shadows; a suitable shader is provided in the repo)
- `Max Distance (float)`: if user drags over that distance, the bond will break
- `Derive Distance From Mass`: if true, will try set the `Max Distance` as a proportion of mass. Mass will be derived from the rigidbody, if there is one, or from the object's volume (assuming density is 1, so a 1m object will weight 100k); To be able to lift heavier objects, dial the `strength` property.
- `Mouse Horizon (float)`: Where does the ray cast from the mouse stops detecting objects
- `Restrict`: allows to restrict movement to the X, Y, or Z plane
	- `x (bool)`: restricts to x axis
	- `y (bool)`: restricts to y axis
	- `z (bool)`: ...

-----
## Auxiliary Classes

### Line.cs
Draws a line  
Properties

- `width (float)`: sets the line width
- `material (Material)`: sets the line material
- `pointA (Vector3)`: sets the first point of the line
- `pointB (Vector3)`: sets the second point of the line
- `distance`: gets the distance between the two points, returns a float
- `visible (bool)`: makes the line visible or invisible

Methods

- `void Enable()`: enables the line (alternative to line.visible=true)
- `void Disable()`: disables the line (alternative to line.visible=false)
- `void SetPoints(Vector3 a, Vector3 b)`: Sets the two points (alternative to using line.pointA, line.pointB)

### RestrictDimension.cs

A simple class that stores x, y, and z booleans. Used mainly to display the restrict options of Draggable and SimpleDraggable in the UI in a pleasant manner.

Properties

- `x`: boolean
- `y`: boolean
- `z`: boolean

-----
## Optional Goodies
They're all under /optionalStuff
### SolidColor.shader
A simple shader that takes a color and transparency. Useful for UI objects that should not project shadows nor receive any.

### FollowMouse.cs
A simple script that makes an object follow the mouse. Set it on the object that you want to use. It will follow the mouse on X and Z axis, leaving the Y intact. Useful for setting a light that follows the mouse, for example.

properties

- `Max Distance (float)`: The distance after which the object stops following the mouse
- `Smooth (float)`: Smoothness of the movement. Lower is smoother.

### KeyboardOrbit.cs
A modified version of KeyboardOrbit.cs found on the [unity wiki](http://wiki.unity3d.com/index.php/Scripts/Controllers)  
Set it on a camera, then click on an object to focus on it, WASD to orbit, mousewheel to zoom in and out.

Properties:

- `Layer Mask (mask)`: Which objects are selectable to rotate around
- `Target (GameObject)`: Start Target to rotate around
- `Distance (float)`: default starting distance

Other properties are not very important...The script is quite messy.

## License

MIT