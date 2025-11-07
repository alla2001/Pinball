using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// VR_Setup_Checker - Diagnostic tool to verify VR plunger setup
/// Attach this to the plunger GameObject to check for common issues
/// </summary>
public class VR_Setup_Checker : MonoBehaviour
{
    [Header("Click the button below in Inspector to run diagnostics")]
    [Space(10)]
    public bool runDiagnostics = false;

    private void OnValidate()
    {
        if (runDiagnostics)
        {
            runDiagnostics = false;
            RunDiagnostics();
        }
    }

    [ContextMenu("Run VR Diagnostics")]
    public void RunDiagnostics()
    {
        Debug.Log("=== VR PLUNGER SETUP DIAGNOSTICS ===");

        // Check 1: XR Interaction Manager
        var interactionManager = FindAnyObjectByType<XRInteractionManager>();
        if (interactionManager == null)
        {
            Debug.LogError("❌ NO XR INTERACTION MANAGER FOUND! Add one via GameObject > XR > Interaction Manager");
        }
        else
        {
            Debug.Log("✓ XR Interaction Manager found");
        }

        // Check 2: XR Origin
        var xrOrigin = FindAnyObjectByType<Unity.XR.CoreUtils.XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogError("❌ NO XR ORIGIN FOUND! Add one via GameObject > XR > XR Origin (VR)");
        }
        else
        {
            Debug.Log("✓ XR Origin found");
        }

        // Check 3: Collider
        var collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("❌ NO COLLIDER on plunger! Add a Box Collider or Capsule Collider");
        }
        else
        {
            Debug.Log($"✓ Collider found: {collider.GetType().Name}");
            if (collider.isTrigger)
            {
                Debug.LogWarning("⚠️ Collider is set to TRIGGER! Uncheck 'Is Trigger' for grabbing to work");
            }
            else
            {
                Debug.Log("✓ Collider is NOT a trigger (correct)");
            }
        }

        // Check 4: XRGrabInteractable
        var grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogError("❌ NO XRGrabInteractable component! The VR_Plunger_Grab script should add this automatically");
        }
        else
        {
            Debug.Log("✓ XRGrabInteractable found");
            Debug.Log($"  - Movement Type: {grabInteractable.movementType}");
            Debug.Log($"  - Interaction Layers: {grabInteractable.interactionLayers.value}");

            if (grabInteractable.movementType == XRBaseInteractable.MovementType.VelocityTracking ||
                grabInteractable.movementType == XRBaseInteractable.MovementType.Instantaneous)
            {
                Debug.LogWarning("⚠️ Movement Type should be 'Kinematic' for plunger control");
            }
        }

        // Check 5: Rigidbody
        var rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("⚠️ NO RIGIDBODY! XRGrabInteractable works better with a Rigidbody (set to Kinematic)");
        }
        else
        {
            Debug.Log($"✓ Rigidbody found - IsKinematic: {rb.isKinematic}");
            if (!rb.isKinematic)
            {
                Debug.LogWarning("⚠️ Rigidbody should be KINEMATIC for manual plunger control");
            }
        }

        // Check 6: VR_Plunger_Grab script
        var vrPlungerGrab = GetComponent<VR_Plunger_Grab>();
        if (vrPlungerGrab == null)
        {
            Debug.LogError("❌ NO VR_Plunger_Grab script found!");
        }
        else
        {
            Debug.Log("✓ VR_Plunger_Grab script found");
            if (vrPlungerGrab.springLauncher == null)
            {
                Debug.LogWarning("⚠️ Spring_Launcher reference is missing in VR_Plunger_Grab!");
            }
        }

        // Check 7: Spring_Launcher
        var springLauncher = GetComponent<Spring_Launcher>();
        if (springLauncher == null)
        {
            Debug.LogError("❌ NO Spring_Launcher script found!");
        }
        else
        {
            Debug.Log("✓ Spring_Launcher script found");
        }

        // Check 8: Controllers
        var directInteractors = FindObjectsByType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>(FindObjectsSortMode.None);
        var rayInteractors = FindObjectsByType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>(FindObjectsSortMode.None);

        if (directInteractors.Length == 0 && rayInteractors.Length == 0)
        {
            Debug.LogError("❌ NO XR INTERACTORS FOUND! Your XR Origin needs XRDirectInteractor on the controllers");
        }
        else
        {
            Debug.Log($"✓ Found {directInteractors.Length} Direct Interactor(s) and {rayInteractors.Length} Ray Interactor(s)");

            foreach (var interactor in directInteractors)
            {
                Debug.Log($"  - Direct Interactor: {interactor.name}, Layers: {interactor.interactionLayers.value}");
            }
        }

        Debug.Log("=== DIAGNOSTICS COMPLETE ===");
    }

    private void Start()
    {
        // Auto-run diagnostics on start in Play mode
        if (Application.isPlaying)
        {
            Invoke("RunDiagnostics", 1f);
        }
    }
}
