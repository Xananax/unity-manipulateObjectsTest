# Unity Move Objects Test

Here are a few scripts to move objects around.

Mainly you'll find:

## Draggable.cs

Attach to any object in the scene to make Rigidbodies draggable.  
Break Force (speed) and Break Torque (fast change of direction) that can break the joint between the mouse and the object are dependant on the object's mass that is set in it's Rigibody.

#### Properties

- `Attach to Center of Mass`: if true, will attach to the center of the object instead of click coordinates
- `Layer Mask`: Use to restrict to certain layers
- `Do Not Break`: Do not break link between mouse and object, even if strong forces are applied
- `Draw Line`: if true, will draw a line between the object and the mouse
- `Spring`: How springy the joint is
- `Damper`: How much dampness does the joint uses
- `Distance`: The distance between the mouse and the clicked point
- `Smooth`: How smooth the movement is. 1 is very smooth, 10 follows the mouse almost exactly
- `Break Force Modifier`: gets added to the automatically calculated Break Force. Set it to minus to make the Joint stronger
- `Break Torque Modifier`: gets added to the automatically calculated Break Torque. Set it to minus to make the joint stronger
- `Strength`: How strong the mouse is. The higher this value, the easier it will be to lift heavy objects
- `Line Width`: The width of the line between the object and the mouse
- `Line Material`: The material of the line (it is advised to set it to something self-illuminated and without shadows; a suitable shader is provided in the repo)
- `Restrict`: allows to restrict movement to the X, Y, or Z plane
- `Mouse Horizon`: Where does the ray cast from the mouse stops detecting objects

-----
## SimpleDraggable.cs
-----
## Auxiliary Classes

### Line.cs
Draws a line  
Properties

- `width`: sets the line width
- `material`: sets the line material
- `pointA`: sets the first point of the line
- `pointB`: sets the second point of the line
- `distance`: gets the distance between the two points
- `visible`: makes the line visible or invisible

Methods

- `Enable()`: enables the line (alternative to line.visible=true)
- `Disable()`: disables the line (alternative to line.visible=false)
- `SetPoints(Vector3 a, Vector3 b)`: Sets the two points (alternative to using line.pointA, line.pointB)

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

- `Max Distance`: The distance after which the object stops following the mouse
- `Smooth`: Smoothness of the movement. Lower is smoother.

### KeyboardOrbit.cs
A modified version of KeyboardOrbit.cs found on the [unity wiki](http://wiki.unity3d.com/index.php/Scripts/Controllers)  
Set it on a camera, then click on an object to focus on it, WASD to orbit, mousewheel to zoom in and out.

Properties:

- `Layer Mask`: Which objects are selectable to rotate around
- `Target`: Start Target to rotate around
- `Distance`: default starting distance

Other properties are not very important...The script is quite messy.

## License

MIT