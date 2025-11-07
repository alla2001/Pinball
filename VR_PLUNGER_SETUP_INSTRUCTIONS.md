# VR Plunger Setup Instructions

This guide will help you set up the VR-grabbable ball launcher (plunger) for your pinball machine.

## Prerequisites

Your project already has the necessary packages installed:
- XR Interaction Toolkit 3.2.1
- OpenXR 1.15.1
- Meta OpenXR 2.3.0
- XR Hands 1.6.1

## Step 1: Set Up XR Origin (If Not Already Done)

1. **Create XR Origin Rig:**
   - In Unity, go to `GameObject > XR > XR Origin (VR)`
   - This creates a complete VR camera rig with controllers
   - Position it at your desired starting position in the scene

2. **Configure XR Interaction Manager:**
   - If not already present, add `GameObject > XR > Interaction Manager`
   - This is required for all XR interactions to work

## Step 2: Configure the Plunger GameObject

1. **Locate the Plunger:**
   - Find your plunger GameObject in the scene hierarchy
   - It should already have the `Spring_Launcher` script attached

2. **Add VR_Plunger_Grab Script:**
   - Select the plunger GameObject
   - Click `Add Component`
   - Search for "VR_Plunger_Grab" and add it
   - The script will automatically add an `XRGrabInteractable` component

3. **Configure VR_Plunger_Grab Settings:**
   - **Spring Launcher:** Should auto-populate with the Spring_Launcher component
   - **Max Pull Distance:** Set to `0.6` (matches the original plunger range)
   - **Min Pull Threshold:** Set to `0.01` (minimum distance before tracking starts)
   - **Force Multiplier:** Set to `1.0` (adjust for stronger/weaker pulls)

## Step 3: Add Collider for Grabbing

The plunger needs a collider so the VR controller can detect and grab it:

1. **Add Box Collider (if not present):**
   - Select the plunger GameObject
   - Click `Add Component > Box Collider`
   - Adjust the size to match the grabbable part of the plunger
   - **Important:** Make sure "Is Trigger" is UNCHECKED

2. **Alternative - Use Capsule Collider:**
   - If the plunger is cylindrical, use `Capsule Collider` instead
   - Adjust radius and height to fit the plunger handle

## Step 4: Configure XRGrabInteractable Component

The `VR_Plunger_Grab` script automatically adds this, but you may want to customize:

1. **Interaction Layer Mask:**
   - Set to `Default` or create a custom layer for pinball elements

2. **Movement Type:**
   - Set to `Kinematic` (we control the movement via script)

3. **Attach Transform:**
   - Leave as default (the script handles positioning)

4. **Throw on Detach:**
   - Set to `false` (we don't want to throw the plunger)

## Step 5: Test in VR

1. **Enable VR Preview:**
   - Make sure your VR headset is connected
   - In Unity, go to `Window > XR > OpenXR > OpenXR Project Validation`
   - Fix any validation issues

2. **Play Mode:**
   - Enter Play mode
   - Use your VR controller to reach out and grab the plunger
   - Pull it back and release to launch the ball

## Troubleshooting

### Plunger won't grab:
- Check that the collider is properly sized and not a trigger
- Verify XR Interaction Manager exists in the scene
- Check Interaction Layer Masks match between controller and plunger

### Plunger doesn't pull back smoothly:
- Adjust the `Max Pull Distance` in VR_Plunger_Grab
- Check that the plunger's parent transform is set correctly
- Verify the Spring_Launcher component is assigned

### Ball doesn't launch with enough force:
- Increase the `Force Multiplier` in VR_Plunger_Grab
- Adjust `_Spring_Force` in the Spring_Launcher component
- Try pulling the plunger further back

### Camera doesn't follow properly in VR:
- The original camera system may conflict with VR camera
- You may need to disable the `Camera_Movement` script when in VR mode
- Consider adding a VR mode toggle to the Manager_Game script

## Optional Enhancements

### Add Haptic Feedback:
Add haptic feedback when grabbing and releasing the plunger for better immersion.

### Visual Feedback:
- Add a highlight or outline shader when the controller hovers over the plunger
- Use XR Interaction Toolkit's `XRInteractableAffordanceStateProvider` for hover effects

### Hand Tracking Support:
Since you have XR Hands installed, you can enable hand tracking:
- The plunger will work with hand tracking automatically
- Pinch gesture will trigger the grab

## Additional Notes

- The VR system works alongside the existing keyboard/touch controls
- You can switch between VR and traditional controls seamlessly
- All original Spring_Launcher features (auto mode, manual mode, sound effects) still work
- The script maintains compatibility with the existing pinball missions and skillshot system
