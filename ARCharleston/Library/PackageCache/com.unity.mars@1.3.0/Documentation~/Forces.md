# Proxy forces

Proxy forces allow you to express a web of flexible spring-like forces that pull your proxy into its final position. These forces are often a combination of aligning relative to other objects and to surrounding surfaces.
You can use them to model common placement scenarios like calculating how to create a space between objects, or how to pull an object towards and in front of the user.

![Add Forces from Proxy](images/Forces/forces-kettle-sim.png)

The screenshot above shows a kettle that has multiple forces applied. It occupies space, sits on a surface, should be near a vertical wall and horizontal edge, and there is padding (there shouldn't be anything) immediately above and in front of the snout.
The mug occupies space, should be on a surface, and is aligned to the kettle.

The following section presents concepts and workflows around using Proxy forces in your application. It shows the steps to create, configure, and solve arrangements on basic forces, and also includes a real-world example you can use to get started.
After reading this, you should have the necessary knowledge to incorporate Proxy forces into your Unity MARS application.

You can find some [examples](#examples) further down the page, as well as a [video](https://drive.google.com/file/d/1kxRkocirkFOTjwzWTsdaQqq0S2HMBt3j/view).

## Force components
Below are the key concepts, types, and main properties that the Proxy forces system uses:

### Forces (`ProxyForces`)

![ProxyForces](images/Forces/proxy-forces-component.png)

Collects and applies the forces below onto this object. One per GameObject. If you enabled the **Require Forces*** option, set forces to act as starting conditions to filter out invalid starting states.

### Alignment (`ProxyAlignmentForce`)

![ProxyAlignmentForces](images/Forces/proxy-alignmnet-force.png)

Applies a force that aligns this object relative to another object. This supports multiple target relations. Depending on the purpose of your application, they work as follows:

|**Target Relation**|**Description**|
|---|---|
|**Move To And Align With**|Moves and rotates to align with the target object.|
|**Move To And Face**|Moves to the target object and faces it (faces towards the Z axis).|
|**Center In Front Of And Face**|Moves in front of the object and faces it (faces towards the Z axis), regardless of distance.|
|**Scene Initial Relative Pose**|Saves the initial alignment (in the scene) from the target to this object, and applies a force to regain that alignment.|
|**Scene Initial Relative Angle**|Saves the initial angle (in the scene) from the target to this object, and applies a force to regain that angle.|

**Scale forces** control the amount of force applied towards the goal pose. Set this value to 0 to make the force have no effect and 1 to apply the full force.

### Region - Occupancy (`ProxyRegionForceOccupancy`)

Describes how the object occupies space and applies a force to keep it from colliding with other occupied spaces. By default, Unity MARS assigns a unit cube to the region transform and creates this cube as a child of the Proxy. This transform defines the region's shape and pose.

![ProxyRegionForceOccupancy](images/Forces/proxy-region-force-occupancy.png)

**Note**: Because Proxy forces don't depend on Unity's physics system, if you want to change the region transform of any Proxy force, you only have to change the scale of the transform assigned in the **Region Transform** object field.

Padding regions are used to model empty/negative space around the object. They can overlap with other paddings but will collide with occupied regions. If you enable the **Is Padding** option, the Proxy will avoid occupied regions, but will allow overlap with other padded areas.

### Region - Towards (`ProxyRegionForceTowards`)

![ProxyRegionForceTowards](images/Forces/proxy-region-force-towards.png)

Describes a region that is attracted towards other objects (an attach-shape or snap-point). A "towards" region is often used to describe parts of objects that should be in contact with a surface such as a wall, the floor, or both.

You can configure this force using the following parameters:

|**Parameter**|**Description**|
|---|---|
|**Towards Layers**|Filters the attraction to only the selected layers.|
|**Towards Alignment**|Filters the attraction to the set alignment (vertical, horizontal up, etc.)|
|**Towards Edge Only**|Attraction can be only towards the edge of the object. This is useful when defining corner alignments, for example.|
|**Region Transform**|Describes the shape of the attachment or attraction region relative to the proxy.|

### Region - Plane2D (`ProxyRegionForcePlane2D`)

![ProxyRegionForcePlane2D](images/Forces/proxy-region-force-plane-2D.png)

Describes a flat 2D region at the object origin that is attracted towards other planes and provides a way to automatically pull the size and alignment of this region from other **Conditions** on the Proxy.

You can configure this force using the following parameters:

|**Parameter**|**Description**|
|---|---|
|**Keep Match Plane**|When enabled within a plane proxy, only is attracted to the matched plane.|
|**Use Plane Size Condition**|When enabled, gets the region size from a **plane size condition** available at start.|
|**Use Alignment Condition**|When enabled, gets the region's alignment from an **alignment condition** available at start.|
|**Plane Size**|Size of the 2D plane in X and Y/Z axes (can automatically pull from an associated plane size condition).|
|**Plane Alignment**|Defines which planes to align with, like only horizontal up (can automatically pull from an associated alignment condition).|

# Examples
There are two examples below:
* A simple workflow for making an avatar look towards you from a surface
* A larger "Dinner for two" example walkthrough

## Getting started: Making an avatar look towards you from a surface
Proxy forces are Unity MARS components that get added to Proxies. You can achieve different effects, depending on the combination of components you have. Proxy forces are useful for detailed alignments such as objects keeping spacing between each other and aligning relative to nearby edges, walls, and other objects. They are commonly used along with plane conditions to refine the placement within a plane.

This section shows you how to make a simple model attached to a proxy that looks at and follows the camera.

### Setting up a Proxy (before adding forces)
Before using proxy forces, you need to set up a proxy where your avatar will reside. To do this, create a horizontal plane Proxy. You can do this from either the:
* MARS Panel (click the **Horizontal Plane** button).
* Hierarchy view (right-click and select **MARS &gt; Presets &gt; Horizontal Plane** from the contextual menu).

![Getting stated proxy prep](images/Forces/getting-started-proxy-prep.png)

Next, add a model to the **Horizontal Plane** Proxy as a child of the Proxy. Because the **Show Children on Tracking Action** option is enabled, this model will only be visible when there is a match.

![Getting started avatar prep](images/Forces/getting-started-avatar.png)

### Adding forces to the Proxy
Now that you have set up your model, you can start adding forces to it. To have your model avatar face the camera, you need two forces:
* A force that moves the avatar to face towards the camera
* A force that constricts the avatar to the plane the Proxy is attached to

To do this, follow these steps:

1. Select the horizontal plane Proxy.
2. In the **Forces** tab, clic the **Add force** button, then click the **Align to Camera** button.
4. Click **Add force** again, then click the **Snap to Plane 2D** button.

![Getting stated add forces](images/Forces/getting-started-adding-forces.png)

Your Proxy now has three components:
* `ProxyForces`, which configures general force settings such as movement
* `ProxyAlignmentForce`, which aligns your Proxy to the camera
* `ProxyRegionForcePlane2D`, which constrains the avatar to the plane

![Getting stated add forces](images/Forces/getting-started-proxy-components.png)

### Testing created forces
To try out your setup, click the **Play** button and start scanning any loaded environment. Your avatar should get placed on the first scanned plane and look towards the camera.

![Getting stated add forces](images/Forces/getting-started-testing-forces.png)

### Conclusions
If you followed these steps, you should now have a simple Proxy that shows up on any scanned plane. This Proxy contains an avatar that faces towards the camera while being constrained to the plane because of the forces that affect it.

This tutorial uses the **Align to Camera** and **Snap to Plane 2D** forces, but you can also try other forces. For example:
* **Occupied Region**, which keeps the object from overlapping with other objects
* **Snap to Vertical Surfaces**, which defines a region on the object that is attracted to horizontal surfaces (on an object)
* **Align to Other Proxy**, which aligns objects to other objects (for example, a control panel that tries to place itself near your base item).

As mentioned before, forces in Unity MARS are components that can be added to Proxies. You can achieve different effects depending on how you combine these forces. To understand how each component affects your Proxies, see the [Force components](#Force-components) section on this page, which provides a detailed explanation of each force.


## Example: Dinner for two using forces
This tutorial walks you through the setup for placing all the cutlery, pots, and wine for an AR dinner for two, using forces to place all objects.

[![Unity MARS Forces Dinner for Two](images/Forces/forces-dinner-for-two.png)](https://drive.google.com/file/d/1kxRkocirkFOTjwzWTsdaQqq0S2HMBt3j/view)
The thumbnail above links to a video of the "Dinner for two" example that uses Proxy forces in Unity MARS.

### Set up the basic scene environment
Start by creating the basic table setup for the dinner. Add the meshes of a plate for you and your partner, and the surrounding cutlery.

![dinner for two](images/Forces/dinner-for-two-setup-1.png)

After you've set all your meshes, convert them into Proxy planes. To do this, right-click on each of your GameObjects and select **MARS &gt; Turn Into &gt; Proxy Plane** from the contextual menu.

![dinner for two](images/Forces/dinner-for-two-setup-2.png)

Finally, attach Plane conditions to your Proxies on all the GameObjects that will be placed for the dinner. Make sure the plane size condition planes have the maximum size disabled and the minimum size set to cover the base of the object.

![dinner for two](images/Forces/dinner-for-two-setup-3.png)

To be able to move the camera around freely, follow these steps:

1. Create a **Reference Person** GameObject.
2. Convert it to a Proxy plane.
3. Add an **Align to camera** force.
4. Make sure that **Allowed motion** is set to **Move And Rotate Y**, and the **Target Relation** is set to **Move To And Align With**.

![dinner for two](images/Forces/dinner-for-two-setup-4.png)

### Organize the dishes
After having the basic setup of all the plates, add **Align to other proxy** and **Snap to Horizontal Surfaces** forces to the first plate.

![dinner for two](images/Forces/dinner-for-two-setup-5.png)

Set the **Target Proxy** on the first plate to be the **Proxy Reference Person** you created earlier. A green line will appear between the two Proxies.

![dinner for two](images/Forces/dinner-for-two-setup-6.png)

Repeat the steps above for the second plate, but set the **Target Proxy** to be the first plate. Also, make sure  that the **Target Relation** is set to **Scene Initial Relative Pose**.

![dinner for two](images/Forces/dinner-for-two-setup-7.png)

### Configure settings for all the cutlery
For all the cutlery, make sure that **all** the Proxies have the following conditions and forces:

* **Proxy Exclusivity** set to **Shared**, so all proxies can be placed in the same detected plane.
    ![dinner for two](images/Forces/dinner-for-two-setup-8.png)

* **Align to other Proxy** set to the **Proxy Reference Person**, so each GameObject is in the correct position relative to the user.

    ![dinner for two](images/Forces/dinner-for-two-setup-9.png)

* Add an **Alignment Condition** (if it doesn't already exist) and make sure it's set to **Horizontal Up**. Also add a **Snap to Plane 2D** force.

    ![dinner for two](images/Forces/dinner-for-two-setup-10.png)

* Finally, add an **Occupied Region** force (1) to all the Proxies so they don't overlap with each other. To do this, click the **Occupied Region** force. This creates a **Region Occupied** GameObject (2). To set up the occupied volume, modify the **Scale** of the **Region Occupied** GameObject.

     ![dinner for two](images/Forces/dinner-for-two-setup-11.png)

* Click the **Play** button and look for an empty table in the loaded environment. You should see the objects get  placed in front of you.

    ![dinner for two](images/Forces/dinner-for-two-setup-12.png)

### Conclusions

This tutorial covered the steps for placing multiple GameObjects on the surface a user is directly looking at. These GameOBjects accomodate themselves to the surface using forces. As you've seen, forces can be helpful when you want multiple objects to arrange themselves in relation to each other in a given space.

As an exercise, you can disable **Continuous Solve** on all Proxy forces to make the objects stay where they are after they were first placed.

### Troubleshooting this example
If only one object appears at a time, try setting all related Proxies' **Exclusivity** to **Shared** (that is, they all "share" the same surface).

To avoid objects overlapping, add a **Region Occupancy** force to each object above and adjust it.

To have a little room between the plates and cutlery, you can add a **Region Padding** force and adjust the amount of empty space as you prefer.

For reference, the screenshot below shows the common configuration for all the major force types:

![Configured proxies](images/Forces/forces-configured.png)

## Using forces without Proxies
You can use forces in Unity MARS without a Proxy. This is only recommended for advanced users.

To use forces without a Proxy, add a **ProxyForces** component and its associated forces to an empty GameObject instead of a Proxy. The forces will be enabled from the start.

In contrast, forces on a Proxy will only run after their associated Proxy has found a match, and only be applied while the match is active. They will also update the Proxy's pose before it reaches the SetPose and other actions.

## Important considerations regarding Proxy forces
Because Proxy forces don't depend on Unity's physics system, the region transform of any Proxy force is determined by a transform and the scale mandates the bounds of said transform. If you want to modify the bounds of a Proxy force's region transform, you must modify the linked transform's scale to set said volume.

## Troubleshooting / FAQ
* I get "jittery" motion as my camera moves.
    * **Solution:** Disable **Set Pose Action** and set **Follow Match Updates** to false. If the object appears to be jumping around as you're scanning, this might be because the **Set Pose Action** updates the object as the "center" of the detected plane as you move.
* I want two or more objects on the same surface.
    * **Solution:** Set the Proxy's **Exclusivity** to **Shared** rather than **Exclusive**. **Shared** allows multiple Proxies to share the same data plane, but you might want to use **Occupancy Regions** to ensure the Proxies don't overlap. **Reserved** is the default and ensures that only one Proxy at a time can use any particular piece of data. **Read Only** causes objects to be placed without checking if any other Proxies are reserving or sharing that data.
* I want objects to not move after being placed.
    * **Solution:** Disable real-time force solving (set the **Proxy Forces / Continuous Solve** option to false).
* A static object didn't solve into the correct location.
    * **Solution:** The easiest method to fix this is via scripting. Either call `ProxyForces.TrySingleSolve`, or enable and disable `ProxyForces.continuousSolve`. Also, user interaction to correct the pose is often suggested and can be used while continuous solving is enabled.
