using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Unity.VRTemplate
{
    /// <summary>
    /// Handles helmet attachment to the player's head when grabbed and brought close to head
    /// </summary>
    public class HelmetAttachment : MonoBehaviour
    {
        [Header("Attachment Settings")]
        [SerializeField]
        [Tooltip("The XR Origin's camera transform (player's head)")]
        Transform m_HeadTransform;

        [SerializeField]
        [Tooltip("Distance threshold to snap helmet to head")]
        float m_AttachDistance = 0.3f;

        [SerializeField]
        [Tooltip("Position offset from head center")]
        Vector3 m_HeadOffset = Vector3.zero;

        [SerializeField]
        [Tooltip("Rotation offset from head rotation")]
        Vector3 m_RotationOffset = Vector3.zero;

        [Header("Visual Settings")]
        [SerializeField]
        [Tooltip("Renderer to hide when helmet is attached")]
        Renderer m_HelmetRenderer;

        XRGrabInteractable m_GrabInteractable;
        Rigidbody m_Rigidbody;
        Collider m_Collider;

        bool m_IsAttached = false;
        Transform m_OriginalParent;
        Vector3 m_OriginalScale;

        void Awake()
        {
            m_GrabInteractable = GetComponent<XRGrabInteractable>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Collider = GetComponent<Collider>();
            m_OriginalParent = transform.parent;
            m_OriginalScale = transform.localScale;

            if (m_HeadTransform == null)
            {
                // Try to find the main camera (XR Origin Camera)
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                    m_HeadTransform = mainCamera.transform;
                else
                    Debug.LogWarning("HelmetAttachment: No head transform assigned and couldn't find Main Camera!");
            }

            if (m_HelmetRenderer == null)
                m_HelmetRenderer = GetComponentInChildren<Renderer>();
        }

        void OnEnable()
        {
            if (m_GrabInteractable != null)
            {
                m_GrabInteractable.selectExited.AddListener(OnGrabReleased);
            }
        }

        void OnDisable()
        {
            if (m_GrabInteractable != null)
            {
                m_GrabInteractable.selectExited.RemoveListener(OnGrabReleased);
            }
        }

        void Update()
        {
            if (m_IsAttached)
            {
                // Keep helmet attached to head
                UpdateAttachedPosition();
            }
        }

        void OnGrabReleased(SelectExitEventArgs args)
        {
            if (m_HeadTransform == null)
                return;

            // Check if helmet is close enough to head to attach
            float distanceToHead = Vector3.Distance(transform.position, m_HeadTransform.position);

            if (distanceToHead <= m_AttachDistance && !m_IsAttached)
            {
                AttachToHead();
            }
        }

        void AttachToHead()
        {
            m_IsAttached = true;

            // Disable physics and interaction
            if (m_Rigidbody != null)
            {
                m_Rigidbody.isKinematic = true;
                m_Rigidbody.useGravity = false;
            }

            if (m_Collider != null)
                m_Collider.enabled = false;

            if (m_GrabInteractable != null)
                m_GrabInteractable.enabled = false;

            // Parent to head
            transform.SetParent(m_HeadTransform);

            // Hide helmet renderer (player shouldn't see inside of helmet)
            if (m_HelmetRenderer != null)
                m_HelmetRenderer.enabled = false;

            UpdateAttachedPosition();

            Debug.Log("Helmet attached to head!");
        }

        void UpdateAttachedPosition()
        {
            // Apply offset position and rotation
            transform.localPosition = m_HeadOffset;
            transform.localRotation = Quaternion.Euler(m_RotationOffset);
            transform.localScale = m_OriginalScale;
        }

        /// <summary>
        /// Detach helmet from head and make it grabbable again
        /// </summary>
        public void DetachFromHead()
        {
            if (!m_IsAttached)
                return;

            m_IsAttached = false;

            // Re-enable physics and interaction
            if (m_Rigidbody != null)
            {
                m_Rigidbody.isKinematic = false;
                m_Rigidbody.useGravity = true;
            }

            if (m_Collider != null)
                m_Collider.enabled = true;

            if (m_GrabInteractable != null)
                m_GrabInteractable.enabled = true;

            // Show helmet again
            if (m_HelmetRenderer != null)
                m_HelmetRenderer.enabled = true;

            // Unparent from head
            transform.SetParent(m_OriginalParent);

            Debug.Log("Helmet detached from head!");
        }

        /// <summary>
        /// Toggle helmet attachment (for UI buttons or other triggers)
        /// </summary>
        public void ToggleAttachment()
        {
            if (m_IsAttached)
                DetachFromHead();
            else
                AttachToHead();
        }

        void OnDrawGizmosSelected()
        {
            if (m_HeadTransform == null)
                return;

            // Draw attachment range sphere
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(m_HeadTransform.position, m_AttachDistance);

            // Draw attachment point
            Gizmos.color = Color.green;
            Vector3 attachPoint = m_HeadTransform.position + m_HeadTransform.TransformDirection(m_HeadOffset);
            Gizmos.DrawWireSphere(attachPoint, 0.05f);
        }
    }
}
