using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// VR_Plunger_Grab - Allows the player to grab and pull back the pinball plunger in VR
/// Attach this to the plunger GameObject along with an XRGrabInteractable component
/// </summary>
public class VR_Plunger_Grab : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the Spring_Launcher script")]
    public Spring_Launcher springLauncher;

    [Header("Pull Settings")]
    [Tooltip("Maximum distance the plunger can be pulled back (in meters)")]
    public float maxPullDistance = 0.6f;

    [Tooltip("Minimum distance before the plunger starts tracking (in meters)")]
    public float minPullThreshold = 0.01f;

    [Header("Force Settings")]
    [Tooltip("Multiplier for the spring force based on pull distance")]
    public float forceMultiplier = 1.0f;

    [Header("Debug")]
    [Tooltip("Enable debug logging to see hand movement values")]
    public bool showDebugLogs = true;

    [Tooltip("Show debug sphere on controller position")]
    public bool showDebugGizmos = true;

    // Private variables
    private XRGrabInteractable grabInteractable;
    private IXRSelectInteractor currentInteractor;
    private Vector3 grabStartPosition;
    private Vector3 plungerRestPositionWorld; // World space rest position - scale independent
    private Vector3 plungerRestPositionLocal; // Local space rest position
    private bool isPulled = false;
    private float currentPullDistance = 0f;
    private Vector3 debugControllerPos;

    private void Awake()
    {
        // Add Rigidbody if not present (required for XRGrabInteractable)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log("VR_Plunger_Grab: Added Rigidbody component");
        }
        // Configure Rigidbody for manual control
        rb.isKinematic = true;
        rb.useGravity = false;

        // Get or add XRGrabInteractable component
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
            Debug.Log("VR_Plunger_Grab: Added XRGrabInteractable component");
        }

        // Configure XRGrabInteractable for plunger behavior
        // IMPORTANT: Disable tracking so plunger stays constrained to its path
        grabInteractable.movementType = XRBaseInteractable.MovementType.Kinematic;
        grabInteractable.throwOnDetach = false;
        grabInteractable.trackPosition = false;  // Don't follow hand - we control position manually
        grabInteractable.trackRotation = false;  // Don't rotate with hand
        grabInteractable.useDynamicAttach = false; // Don't update attach point

        // Ensure collider is not a trigger
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Debug.LogWarning("VR_Plunger_Grab: Collider should NOT be a trigger for grabbing! Disabling trigger mode.");
            col.isTrigger = false;
        }

        // Find Spring_Launcher if not assigned
        if (springLauncher == null)
        {
            springLauncher = GetComponent<Spring_Launcher>();
        }

        if (springLauncher == null)
        {
            Debug.LogError("VR_Plunger_Grab: Spring_Launcher component not found!");
        }
        else
        {
            Debug.Log("VR_Plunger_Grab: Successfully initialized and linked to Spring_Launcher");
        }
    }

    private void Start()
    {
        // Store BOTH world and local rest positions to handle weird scales
        plungerRestPositionWorld = transform.position;
        plungerRestPositionLocal = transform.localPosition;

        // Make sure plunger starts at rest position
        transform.position = plungerRestPositionWorld;

        Debug.Log($"VR_Plunger_Grab: Rest position stored - World: {plungerRestPositionWorld}, Local: {plungerRestPositionLocal}");
        Debug.Log($"Parent scale: {(transform.parent != null ? transform.parent.lossyScale.ToString() : "No parent")}");
    }

    private void OnEnable()
    {
        // Subscribe to grab events
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        // Unsubscribe from grab events
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;
        grabStartPosition = currentInteractor.GetAttachTransform(grabInteractable).position;
        isPulled = true;

        // IMMEDIATELY lock position to world rest position (avoids scale issues)
        transform.position = plungerRestPositionWorld;
        transform.localRotation = Quaternion.identity;

        Debug.Log($"VR Plunger grabbed! Rest position (world): {plungerRestPositionWorld}");
        Debug.Log($"Current world position: {transform.position}");
        Debug.Log($"Grab world pos: {grabStartPosition}, Plunger forward: {transform.forward}");
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isPulled = false;
        currentInteractor = null;

        // Release the plunger - it will spring back and launch the ball
        if (currentPullDistance > minPullThreshold)
        {
            ReleasePlunger();
        }

        Debug.Log($"VR Plunger released! Pull distance: {currentPullDistance}");
    }

    private void Update()
    {
        if (isPulled && currentInteractor != null)
        {
            UpdatePlungerPosition();
        }
    }

    private void LateUpdate()
    {
        // Force position lock AFTER all other systems have updated
        // Working in WORLD SPACE to avoid scale issues
        if (isPulled)
        {
            // Keep rotation locked
            transform.localRotation = Quaternion.identity;

            // Lock position to the pull rail in world space
            // Calculate where plunger should be along its forward direction
            Vector3 pullDirection = -transform.forward; // Direction to pull
            Vector3 targetWorldPos = plungerRestPositionWorld + (pullDirection * currentPullDistance);

            transform.position = targetWorldPos;
        }
    }

    private void UpdatePlungerPosition()
    {
        // ALL CALCULATIONS IN WORLD SPACE to avoid scale issues

        // Get current controller position (world space)
        Vector3 currentControllerPos = currentInteractor.GetAttachTransform(grabInteractable).position;
        debugControllerPos = currentControllerPos;

        // Calculate movement in world space
        Vector3 worldMovement = currentControllerPos - grabStartPosition;

        // Get plunger's pull direction (opposite of forward)
        Vector3 pullDirection = -transform.forward;

        // Project the hand movement onto the plunger's pull direction
        float pullDistance = Vector3.Dot(worldMovement, pullDirection);

        // Clamp to valid range (0 to maxPullDistance)
        pullDistance = Mathf.Clamp(pullDistance, 0f, maxPullDistance);
        currentPullDistance = pullDistance;

        if (showDebugLogs && Time.frameCount % 30 == 0) // Log every 30 frames to avoid spam
        {
            Debug.Log($"Hand moved: {worldMovement.magnitude:F3}m | Pull distance: {pullDistance:F3}m");
            Debug.Log($"World pos: {transform.position}");
        }

        // Position is updated in LateUpdate to prevent conflicts

        // Notify Spring_Launcher that plunger is being pulled
        if (springLauncher != null && pullDistance > minPullThreshold)
        {
            springLauncher.springIsPulled = true;
        }
    }

    private void ReleasePlunger()
    {
        if (springLauncher == null) return;

        // Calculate force based on pull distance
        float normalizedPull = currentPullDistance / maxPullDistance;

        Debug.Log($"Releasing plunger! Pull: {currentPullDistance:F3}m, Normalized: {normalizedPull:F3}");

        // Trigger the spring launcher with VR pull force
        springLauncher.VR_LaunchBall(normalizedPull * forceMultiplier);

        // Reset pull distance
        currentPullDistance = 0f;

        // Reset position to world rest position (scale independent)
        transform.position = plungerRestPositionWorld;
    }

    // Optional: Visualize the pull distance in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position - transform.forward * maxPullDistance;
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawWireSphere(endPos, 0.02f);
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos || !Application.isPlaying || !isPulled) return;

        // Draw controller position
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(debugControllerPos, 0.03f);

        // Draw grab start position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(grabStartPosition, 0.02f);

        // Draw line between them
        Gizmos.color = Color.white;
        Gizmos.DrawLine(grabStartPosition, debugControllerPos);

        // Draw plunger's forward direction
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.forward * 0.2f);
    }
}
