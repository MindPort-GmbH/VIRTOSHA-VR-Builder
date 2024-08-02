# VIRTOSHA for VR Builder Manual

## Drilling holes in game objects

### Introduction
This feature allows the user to dynamically drill holes in game objects by using an appropriate tool. The holes are not three-dimensional (that is, they won't alter the object's geometry). It is also possible to specify points on the game object where holes should be drilled, and a VR Builder condition that checks for holes drilled at specific positions is provided.

What follows is a detailed description of the components of this feature.

### The Drillable Property
The `Drillable Property` component defines an object that can be drilled holes into. Since it is a VR Builder property, it is possible to interact with it from behaviors and conditions, like for example the provided `Drilled Holes Condition`.
Holes are added to the drillable property only when the drilling is finished. Each hole has a start point (usually on the surface of the object), a end point and a width.

The `Debug Display Holes` option in the component's inspector, if checked, will show all drilled holes on the object as wireframe cylinders in the scene view.

Note that while this property concerns itself with storing data about the drilled holes, it does not provide any visual representation of the holes in the Game window. Adding the `Debug Hole Representation` component to the same game object will provide a basic representation for debug purposes: a black sphere representing the hole, and a text stating the depth in millimeters. A custom visual representation should be provided depending on the use case.

### The Drill
This feature provides the necessary components to create an interactable drill object, that can be used to drill holes in Drillable Properties. To use the drill, hold the trigger button of the controller you are grabbing it with, then push the point inside a drillable game object.

Note that, since both the drill and the drillable objects use VR Builder lockable properties, you will not be able to use the drill and drill holes at any point in a process. Both the drill and the drillable objects need to be unlocked for it to happen.

If you want to test drilling in a sandbox environment, disable the `PROCESS_CONTROLLER` game object in the VR Builder scene. This will prevent the process to run and all properties will be unlocked.

There are three components that need to be added to a composite game object.

#### The Drill Component
The `Drill` component should be added to the main interactable body of the drill. Once added to a game object, it will automatically add dependencies to make it interactable in VR and usable in VR Builder. The `Usable Property` for VR Builder is used to determine whether the drill is activated or not. By default, this corresponds to holding the trigger button on the controller while the object is grabbed.

There are two parameters on the `Drill` component.

**Max Deviation**: When the drill starts creating a hole in an object, it determines a starting point and a direction. It will keep drilling deeper as long as the drill is moved along the original direction. This value determines the distance from the original vector at which the drilling will be automatically stopped.

**Affordance prefab**: The drill provides no graphical representation of the hole being drilled by default, but it will spawn this prefab at the position where it starts drilling a hole. The default prefab, `DebugDrillingAffordance`, will display some particles along with a text representation of the drilled depth in millimeters. You can create your own prefab by adding an implementation of the `DrillingAffordance` component on its root game object.

#### The Drill Bit Component
This component represent the drill bit and is meant to be located on a child game object to the `Drill` component. It should also be in-axis with the drilled hole, ideally at the base of the drill bit with its forward vector pointing towards the point of the drill.

It is theoretically possible to create a system of swappable drill bits, each drilling holes of different sizes.

The `Drill Bit` component has the following parameters.

**Width**: This determines the width of the drill bit, and conversely of the hole that will be drilled.

**Drill Tip**: This is a reference to the `Drill Tip` component (see below). If left empty, it will look for the component in this game object's children.

#### The Drill Tip Component
We need one last component to set up the drill, and that is a component representing the tip. This component is used for detecting when the tip of the drill touches something. This component should be placed on a child of the `Drill Bit` game object. It has no parameters, but a trigger should be present on the same game object. Ideally, use a small sphere collider with the same width as the drill and set it as trigger.

### The Drillable Socket Property
The `Drillable Socket Property` component is another VR Builder property that lets you easily specify where and how you want an object to be drilled.
If you add a `Drillable Socket Property` to an empty game object, a child game object will be automatically created. Now the original object represents the start point of the hole, and the child the end point. A cyan wireframe cylinder is displayed between the two point to represent the hole.
You can drag these two game objects around in order to easily place the hole where you see fit. Additionally, you can tweak the following parameters on the component itself.

**Width**: The desired width of the hole. This is reflected by the width of the wireframe cylinder.

**Enter Tolerance**, **End Tolerance**: How close the enter point and end point of a hole must be from the corresponding points on the socket in order for the hole to be considered the same as the socket. These tolerances are displayed as yellow wireframe spheres in the Scene view.

**Width Tolerance**: How much a hole's width can diverge to the specified width of the socket in order to be considered the same as the socket.

**Place Enter Point On Drillable Object Surface**: Instead of trying to place the socket exactly on a drillable object's surface, it may be better to let it stick out a bit and check this option. While this is checked, if the socket intersects a drillable object, the socket will report the enter point of the hole as a point on the surface of the drillable object instead of its own origin. This is reflected by the tolerance sphere, which will move on the surface of the object.
Note that not having the enter point near the surface of the object might cause the hole not to be detected, as the drill creates holes that start from the surface of the object.

**Show Highlight Object**: If checked, a highlight showing where to drill will be displayed while this property is unlocked. This is represented by a cylinder with the shape of the socket.

**Highlight Material**: The material used by the highlight object. Note that at the moment there is no default setting for this material - if not specified, a default white mesh will be created.

### The Drill Holes Condition
Finally, we can put everything together with the Drill Holes condition for VR Builder. This condition checks that a number of holes is drilled at specific positions on a drillable object. For this, it only needs two parameters.

**Drillable object**: The object to be drilled.

**Drillable sockets**: The drillable socket object that specify where and how the object should be drilled. Note that this can be a single object, or one or more object groups. If multiple sockets are selected, all need to be drilled in order for the condition to succeed.

Note that the drill is not referenced in the condition. That is because the user is free to drill the hole however they want. But this will mean that the drill is not automatically unlocked, and therefore will not be grabbable and usable. You can rectify this by manually adding the tool you intend to use to drill the holes to the `Unlocked Objects` tab of the step.

If the Drill Holes condition is fast forwarded, the holes will be created automatically.