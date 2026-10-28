# VR Cube Challenge

## 1. AI Usage Declaration

To develop the **VR Cube Challenge** project, we used **Claude (Anthropic)** as a pedagogical and support tool throughout the process. The AI was mainly used as a guide to explain and help us understand new concepts, such as Unity's configuration for virtual reality, setting up the environment for Meta Quest 3, explaining VR scripts and interactions, and resolving theoretical questions or specific connection and build errors.

The team took full charge of the project logic, scene setup, reference assignments in Unity's Inspector, building for Android with OpenXR, and performing interaction tests directly on the headset.

## 2. Brief Implementation Overview

The project was developed in **Unity 6**, using the **VR template**, **XR Origin**, and **XR Interaction Manager**. It was configured for **Android with OpenXR** and executed on a **Meta Quest 3** using the **Build And Run** option. The left and right controllers are assigned in the Inspector and serve as references for position and hand orientation.

### Grabbing, Releasing, and Throwing the Cube — `CustomVRGrab.cs`

The script gets the cube's `Rigidbody` component and checks whether the **grip** button is pressed on either controller. If a hand is within **0.2 meters** of the cube and the grip is pressed, the cube is grabbed: its `Rigidbody` switches to `isKinematic` mode, and the object tracks the hand's position and rotation using `MovePosition` and `MoveRotation`.

While the cube is grabbed, the script calculates the hand's velocity by comparing its current position to the previous frame. Upon releasing the grip, physics are re-enabled, and the cube is assigned that velocity multiplied by **1.2**, allowing it to be thrown.

### Shooting Spheres and Object Management — `VRShooter.cs`

The script checks controller inputs using `UnityEngine.XR`. Pressing the **trigger** instantiates a sphere using `GameObject.CreatePrimitive`, places it in front of the hand, assigns a random size and color, adds a `Rigidbody`, and propels it forward in the direction the controller is pointing. The sphere is automatically destroyed after the duration specified in `sphereLifetime`.

Pressing **A** on the right controller or **X** on the left controller destroys scene objects containing a `Rigidbody`, except those belonging to `protectedRoot` (a reference used to protect the XR Origin). This feature does not delete literally all `GameObjects`; for example, the floor and lights without a `Rigidbody` remain intact.

As an extra feature, pressing **B** (right controller) or **Y** (left controller) allows the user to respawn the cube. At startup, the script saves a disabled copy of the cube and uses it to clone a new one whenever requested.

The auxiliary function `Edge` detects the moment a button transitions from unpressed to pressed, preventing spheres from continuously firing or triggering actions repeatedly while holding the button down.
