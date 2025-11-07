using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// VR_Flipper_Button - A grabbable button that stays in place and activates flippers when trigger is pressed
/// Attach this to a small grabbable object near each flipper
/// The object should be positioned where the player's hand would naturally rest (like a cabinet button)
/// </summary>
public class VR_Flipper_Button : MonoBehaviour
{
    [Header("Flipper Reference")]
    [Tooltip("The Flippers script to control")]
    public Flippers flipperScript;

    [Header("Button Settings")]
    [Tooltip("Visual feedback object that moves when trigger is pressed (optional)")]
    public Transform buttonVisual;

    [Tooltip("How far the visual button moves when pressed (in local Z)")]
    public float buttonPressDepth = 0.01f;

    [Header("Haptic Feedback")]
    [Tooltip("Enable haptic feedback when button is pressed")]
    public bool useHaptics = true;

    [Tooltip("Haptic intensity (0-1)")]
    public float hapticIntensity = 0.5f;

    [Tooltip("Haptic duration in seconds")]
    public float hapticDuration = 0.1f;

    [Header("Debug")]
    [Tooltip("Show debug logs")]
    public bool showDebugLogs = false;

    // Private variables
    private XRGrabInteractable grabInteractable;
    private IXRSelectInteractor currentInteractor;
    private bool isGrabbed = false;
    private bool isTriggerPressed = false;
    private Vector3 buttonRestPosition;

    private void Awake()
    {
        // Add Rigidbody if not present (required for XRGrabInteractable)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        // Configure Rigidbody - button doesn't move
        rb.isKinematic = true;
        rb.useGravity = false;

        // Get or add XRGrabInteractable component
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
        }

        // Configure XRGrabInteractable for button behavior
        grabInteractable.movementType = XRBaseInteractable.MovementType.Kinematic;
        grabInteractable.throwOnDetach = false;
        grabInteractable.trackPosition = false;  // Button stays in place
        grabInteractable.trackRotation = false;  // Button doesn't rotate
        grabInteractable.useDynamicAttach = false;

        // Ensure collider is not a trigger
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            col.isTrigger = false;
        }

        // Store button visual rest position
        if (buttonVisual != null)
        {
            buttonRestPosition = buttonVisual.localPosition;
        }
    }

    private void OnEnable()
    {
        // Subscribe to grab/release events
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);

        // Subscribe to activate/deactivate events (trigger press)
        grabInteractable.activated.AddListener(OnTriggerPressed);
        grabInteractable.deactivated.AddListener(OnTriggerReleased);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
        grabInteractable.activated.RemoveListener(OnTriggerPressed);
        grabInteractable.deactivated.RemoveListener(OnTriggerReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;
        isGrabbed = true;

        if (showDebugLogs)
        {
            Debug.Log($"VR Flipper Button grabbed! Now press TRIGGER to activate flipper.");
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        currentInteractor = null;

        // Make sure flipper is deactivated when button is released
        if (isTriggerPressed)
        {
            DeactivateFlipper();
        }

        if (showDebugLogs)
        {
            Debug.Log($"VR Flipper Button released!");
        }
    }

    private void OnTriggerPressed(ActivateEventArgs args)
    {
        if (!isGrabbed || flipperScript == null) return;

        isTriggerPressed = true;
        ActivateFlipper();

        // Visual feedback - push button in
        if (buttonVisual != null)
        {
            Vector3 pressedPos = buttonRestPosition;
            pressedPos.z -= buttonPressDepth;
            buttonVisual.localPosition = pressedPos;
        }

        // Haptic feedback
        if (useHaptics && currentInteractor != null)
        {
            SendHapticFeedback();
        }

        if (showDebugLogs)
        {
            Debug.Log($"TRIGGER PRESSED - Flipper activated!");
        }
    }

    private void OnTriggerReleased(DeactivateEventArgs args)
    {
        if (!isGrabbed || flipperScript == null) return;

        isTriggerPressed = false;
        DeactivateFlipper();

        // Visual feedback - release button
        if (buttonVisual != null)
        {
            buttonVisual.localPosition = buttonRestPosition;
        }

        if (showDebugLogs)
        {
            Debug.Log($"TRIGGER RELEASED - Flipper deactivated!");
        }
    }

    private void ActivateFlipper()
    {
        if (flipperScript != null)
        {
            flipperScript.ActivateFlipper();
        }
    }

    private void DeactivateFlipper()
    {
        if (flipperScript != null)
        {
            flipperScript.DeactivateFlipper();
        }
    }

    private void SendHapticFeedback()
    {
        // Check if the interactor supports haptics
        if (currentInteractor is XRBaseInputInteractor controllerInteractor)
        {
            var controller = controllerInteractor.xrController;
            if (controller != null)
            {
                controller.SendHapticImpulse(hapticIntensity, hapticDuration);
            }
        }
    }

    // Visualize the button in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.03f);

        // Draw a line showing the button press direction
        if (buttonVisual != null)
        {
            Gizmos.color = Color.green;
            Vector3 start = buttonVisual.position;
            Vector3 end = buttonVisual.position - buttonVisual.forward * buttonPressDepth;
            Gizmos.DrawLine(start, end);
        }
    }
}
