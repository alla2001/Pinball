# VR Flipper Button Setup Instructions

This guide will help you set up VR grabbable buttons to control your pinball flippers. Each flipper gets its own button that players grab and press the trigger to activate.

## Concept

Instead of directly grabbing the flippers (which would be awkward), players grab "button" objects positioned like pinball cabinet buttons. When holding a button and pressing the VR controller's trigger, the flipper activates - just like pressing physical flipper buttons!

## Setup Steps

### Step 1: Create the Left Flipper Button

1. **Create a button GameObject:**
   - Right-click in Hierarchy > `3D Object > Cube` (or Sphere/Cylinder)
   - Rename it to `VR_Button_Left_Flipper`

2. **Position the button:**
   - Place it where the player's left hand would naturally rest
   - Typical position: Lower left side of the pinball table, at a comfortable height
   - Scale it to button size: Around `0.05, 0.05, 0.05` (5cm cube)

3. **Add the VR_Flipper_Button script:**
   - Select `VR_Button_Left_Flipper`
   - Click `Add Component`
   - Search for "VR_Flipper_Button" and add it

4. **Link to the left flipper:**
   - In the VR_Flipper_Button component:
     - Drag your **left flipper GameObject** into the `Flipper Script` field
     - This should have the `Flippers` component with `b_Flipper_Left = true`

5. **Configure settings:**
   - `Use Haptics`: Checked (gives tactile feedback)
   - `Haptic Intensity`: 0.5
   - `Show Debug Logs`: Checked (for testing)

### Step 2: Create the Right Flipper Button

Repeat the same process for the right flipper:

1. **Create button:**
   - `3D Object > Cube`
   - Rename to `VR_Button_Right_Flipper`

2. **Position:**
   - Lower right side of the table
   - Mirror the left button's position

3. **Add script:**
   - Add Component > VR_Flipper_Button

4. **Link to right flipper:**
   - Drag your **right flipper GameObject** into `Flipper Script`
   - This should have `Flippers` component with `b_Flipper_Right = true`

### Step 3: Optional - Add Visual Button Press

For better feedback, you can make the button visually press down:

1. **Create a visual child object:**
   - Right-click on `VR_Button_Left_Flipper` > `3D Object > Cube`
   - Rename to `Button_Visual`
   - Scale slightly smaller than parent: `0.8, 0.8, 0.8`
   - Add a different material/color to make it stand out

2. **Link the visual:**
   - Select `VR_Button_Left_Flipper`
   - In VR_Flipper_Button component:
     - Drag `Button_Visual` into the `Button Visual` field
     - Set `Button Press Depth`: 0.01 (1cm)

3. **Repeat for right button**

### Step 4: Verify Setup

Check each button has:
- ✓ VR_Flipper_Button script
- ✓ Flipper Script field assigned to correct flipper
- ✓ Collider (Box Collider should be added automatically)
- ✓ Rigidbody with IsKinematic = true (added automatically)
- ✓ XRGrabInteractable component (added automatically)

## Testing in VR

1. **Enter Play Mode** with VR headset

2. **Test Left Flipper:**
   - Reach out and grab the left button (grip button)
   - While holding, press the **trigger** on your controller
   - The left flipper should activate!
   - Release trigger - flipper should deactivate

3. **Test Right Flipper:**
   - Same process with the right button

4. **Check Console:**
   - You should see: "TRIGGER PRESSED - Flipper activated!"
   - And: "TRIGGER RELEASED - Flipper deactivated!"

## Positioning Tips

### Where to Place Buttons

Think about where your hands naturally rest when playing pinball:

**Option 1 - Side Buttons (Traditional Cabinet Style):**
```
                [Table]

    [L]                    [R]
   Left                   Right
  Button                 Button
```
- Position: On the sides of the table, about waist height
- Good for: Realistic arcade cabinet feel

**Option 2 - Lower Front Buttons:**
```
          [Table]

    [L]          [R]

   (Player standing here)
```
- Position: Front lower corners, easier to reach
- Good for: Comfortable VR play, less arm movement

**Option 3 - Floating Buttons:**
```
          [Table]
    [L]            [R]

   (Buttons float in comfortable positions)
```
- Position: Wherever feels most natural in VR
- Good for: Maximum comfort, not restricted by table geometry

### Size Recommendations

- **Button size**: 5-8cm (0.05 - 0.08 scale)
- **Distance apart**: Roughly shoulder width
- **Height**: Natural hand resting position (usually table height or slightly below)

## Advanced Configuration

### Haptic Feedback

Fine-tune the feel:
- **Intensity**: 0.3 = gentle buzz, 0.7 = strong vibration
- **Duration**: 0.05s = quick tap, 0.2s = noticeable pulse

### Visual Feedback

Make buttons more obvious:
- Add emissive materials that glow when grabbed
- Use bright colors: Green for left, Red for right
- Add labels/text above buttons

### Button Appearance Ideas

1. **Simple Cube** - Clean, minimal
2. **Cylinder** - Looks like arcade button
3. **Sphere** - Easy to grab from any angle
4. **Custom Model** - Import a realistic button model

## Troubleshooting

### Button won't grab:
- Check that collider exists and is NOT a trigger
- Verify XR Direct Interactor is on controllers
- Check Interaction Layer Masks match

### Flipper doesn't activate when trigger pressed:
- Verify `Flipper Script` field is assigned
- Check that the flipper's `Activate` is true in Inspector
- Look for errors in Console
- Make sure flipper has `Flippers` component

### Flipper stays active after releasing trigger:
- Check that `deactivated` event is firing (enable Show Debug Logs)
- Verify flipper script's `DeactivateFlipper()` is working

### Button moves when grabbed:
- Check that `trackPosition = false` in XRGrabInteractable
- Verify Rigidbody is set to IsKinematic

### Wrong flipper activates:
- Check you assigned the correct flipper to each button
- Left button should control left flipper (b_Flipper_Left = true)
- Right button should control right flipper (b_Flipper_Right = true)

## Button Hierarchy Example

Your setup should look like this:

```
Pinball Machine
├── Flippers
│   ├── Flipper_Left (has Flippers script, b_Flipper_Left = true)
│   └── Flipper_Right (has Flippers script, b_Flipper_Right = true)
├── VR Controls
│   ├── VR_Button_Left_Flipper
│   │   ├── VR_Flipper_Button (linked to Flipper_Left)
│   │   ├── Box Collider
│   │   ├── Rigidbody (Kinematic)
│   │   ├── XRGrabInteractable
│   │   └── Button_Visual (optional child)
│   └── VR_Button_Right_Flipper
│       ├── VR_Flipper_Button (linked to Flipper_Right)
│       ├── Box Collider
│       ├── Rigidbody (Kinematic)
│       ├── XRGrabInteractable
│       └── Button_Visual (optional child)
```

## Using Both Keyboard and VR

Good news! The VR buttons work alongside existing controls:
- ✓ Keyboard controls still work
- ✓ Touch controls still work
- ✓ VR trigger controls work
- All control methods can be used simultaneously

## Tips for Best Experience

1. **Test button positions extensively** - Comfort is key in VR
2. **Add visual indicators** - Make buttons obvious and easy to find
3. **Consider asymmetric positions** - Match real pinball cabinet layouts
4. **Enable haptics** - Physical feedback helps immersion
5. **Use distinct colors** - Help players identify left vs right quickly

## Next Steps

Once flippers work, you might want to add:
- VR controls for tilt (shake/push the table)
- VR controls for launching new balls
- Hand presence visualization
- Score display visible in VR
