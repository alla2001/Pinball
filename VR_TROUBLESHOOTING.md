# VR Plunger Troubleshooting Guide

## Quick Fix Steps

### Step 1: Run the Diagnostic Tool

1. **Attach the diagnostic script:**
   - Find your plunger GameObject in the hierarchy
   - Add Component > VR_Setup_Checker
   - Right-click on the component header and select "Run VR Diagnostics"
   - Check the Console window for error messages

### Step 2: Fix Common Issues

Based on the diagnostic output, follow these fixes:

#### ❌ "NO XR INTERACTION MANAGER FOUND"
**Fix:**
- Go to `GameObject > XR > Interaction Manager`
- This creates an XR Interaction Manager in your scene
- Make sure it stays in the scene (don't delete it)

#### ❌ "NO XR ORIGIN FOUND"
**Fix:**
- Go to `GameObject > XR > XR Origin (VR)`
- This creates the full VR camera rig with controllers
- Position it where you want the player to start

#### ❌ "NO COLLIDER on plunger"
**Fix:**
- Select the plunger GameObject
- Add Component > Box Collider (or Capsule Collider)
- Adjust the size to cover the grabbable part (handle)
- Make sure "Is Trigger" is UNCHECKED

#### ⚠️ "Collider is set to TRIGGER"
**Fix:**
- Select the plunger GameObject
- In the Collider component, UNCHECK "Is Trigger"
- Grab interactions require solid colliders, not triggers

#### ❌ "NO XR INTERACTORS FOUND"
**Fix:**
This is the most common issue! Your controllers need interactor components:

1. **Find the controllers in your XR Origin:**
   - Expand `XR Origin > Camera Offset`
   - You should see `LeftHand Controller` and `RightHand Controller`

2. **Add XR Direct Interactor to BOTH controllers:**
   - Select `LeftHand Controller`
   - Add Component > XR Direct Interactor
   - Select `RightHand Controller`
   - Add Component > XR Direct Interactor

3. **Configure the Direct Interactors:**
   - In XR Direct Interactor component:
     - Interaction Manager: Should auto-link to the scene's XR Interaction Manager
     - Interaction Layer Mask: Set to "Default"
     - Attach Transform: Leave empty (uses controller position)

### Step 3: Verify Plunger Setup

Make sure these are on your plunger GameObject:
- ✓ VR_Plunger_Grab script
- ✓ Spring_Launcher script
- ✓ XRGrabInteractable (added automatically)
- ✓ Rigidbody with IsKinematic = true (added automatically)
- ✓ Collider (Box or Capsule, NOT a trigger)

### Step 4: Check Interaction Layers

If you still can't grab:

1. **On the Plunger:**
   - Select the plunger GameObject
   - Find XRGrabInteractable component
   - Check "Interaction Layer Mask" - should include "Default"

2. **On the Controllers:**
   - Select LeftHand Controller
   - Find XR Direct Interactor component
   - Check "Interaction Layer Mask" - should include "Default"
   - Repeat for RightHand Controller

The layers MUST overlap for grabbing to work!

## Alternative: Use Ray Interactor

If Direct Interactor still doesn't work, try Ray Interactor (pointer-based):

1. **Add XR Ray Interactor to controllers:**
   - Select `LeftHand Controller`
   - Add Component > XR Ray Interactor
   - Select `RightHand Controller`
   - Add Component > XR Ray Interactor

2. **Add Line Visual:**
   - On each XR Ray Interactor:
     - Line Type: Straight Line or Projectile Curve
     - Valid Color Gradient: Set to green
     - Invalid Color Gradient: Set to red

Now you can point at the plunger and use the grip/trigger button to grab it!

## Testing Checklist

Before entering Play mode:

- [ ] XR Interaction Manager exists in scene
- [ ] XR Origin exists and has cameras + controllers
- [ ] Both controllers have XR Direct Interactor (or XR Ray Interactor)
- [ ] Plunger has VR_Plunger_Grab script
- [ ] Plunger has Spring_Launcher script
- [ ] Plunger has a Collider (NOT a trigger)
- [ ] Plunger has XRGrabInteractable component
- [ ] All interaction layers are set to "Default"

## In Play Mode Testing

1. **Enter Play Mode** (with VR headset on)
2. **Look for your controllers** in the scene
3. **Check Console** for VR_Plunger_Grab initialization messages:
   - Should see: "VR_Plunger_Grab: Successfully initialized"
4. **Reach toward the plunger** with your hand
5. **With Direct Interactor:** Controller should detect when close
6. **With Ray Interactor:** Point at plunger (line should turn green)
7. **Press Grip button** to grab

### Debug Messages to Look For:

```
✓ "VR_Plunger_Grab: Successfully initialized and linked to Spring_Launcher"
✓ "VR Plunger grabbed!" (when you grab)
✓ "VR Plunger released! Pull distance: X" (when you release)
```

If you don't see "VR Plunger grabbed!" when pressing grip, the grab isn't registering.

## Still Not Working?

### Issue: Controllers not visible in Play mode
**Solution:**
- Check that OpenXR is enabled in Project Settings > XR Plug-in Management
- For Quest: Enable "Oculus" or "Meta XR"
- For PC VR: Enable "OpenXR"

### Issue: Can grab but plunger doesn't move
**Possible causes:**
- Check that `maxPullDistance` is not 0 in VR_Plunger_Grab
- Verify the plunger isn't constrained by a parent or joint
- Check Console for any error messages during grab

### Issue: Plunger moves wrong direction
**Solution:**
- The script assumes Z-axis is the pull direction
- If your plunger faces a different direction, you may need to adjust the axis in UpdatePlungerPosition()
- Check the yellow gizmo line when plunger is selected - that's the pull direction

### Issue: Not enough force when launching
**Solution:**
- Increase `Force Multiplier` in VR_Plunger_Grab (try 2.0 or 3.0)
- Increase `_Spring_Force` in Spring_Launcher component
- Pull the plunger further back before releasing

## Contact Info

If you've tried all these steps and it's still not working, check:
1. Unity Console for error messages
2. Run the VR_Setup_Checker diagnostic tool
3. Verify your VR device is working in other Unity VR projects

## Quick Setup Recap

**Minimum required setup:**
```
Scene Hierarchy:
├── XR Interaction Manager
├── XR Origin (VR)
│   ├── Camera Offset
│   │   ├── Main Camera
│   │   ├── LeftHand Controller [+ XR Direct Interactor]
│   │   └── RightHand Controller [+ XR Direct Interactor]
└── Pinball Machine
    └── Plunger
        ├── VR_Plunger_Grab
        ├── Spring_Launcher
        ├── XRGrabInteractable
        ├── Rigidbody (Kinematic)
        └── Collider (NOT trigger)
```
