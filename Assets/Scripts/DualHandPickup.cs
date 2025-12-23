using UnityEngine;

public class DualHandPickup : MonoBehaviour
{
    [Header("General")]
    public float pickupRange = 3f;
    public Transform cameraTransform;
    public CameraController cameraController; // 🔥 referensi kamera controller

    [Header("Hand Anchors")]
    public Transform rightHandAnchor;
    public Transform leftHandAnchor;

    // ==== STATIC (FIX FOR GASLOT) ====
    public static Transform RightHand;
    public static Transform LeftHand;

    private Rigidbody heldRight;
    private Rigidbody heldLeft;

    private ObjectHighlight lastHighlight;

    void Start()
    {
        // Set static references
        RightHand = rightHandAnchor;
        LeftHand = leftHandAnchor;

        if (cameraController == null)
        {
            cameraController = FindFirstObjectByType<CameraController>();
        }

        if (cameraTransform == null && cameraController != null)
        {
            cameraTransform = cameraController.mainCamera.transform;
        }
    }

    void Update()
    {
        if (RackInteract.UIActive)
        {
            ClearHighlight();
            InteractionPromptUI.Instance.Hide();
            return; 
        }

        UpdateHighlight();
        HandleHandsInput();
    }

    // ===== Raycast dengan dukungan TPS/FPS =====
    RaycastHit? SmartRaycast()
    {
        RaycastHit hit;

        // ambil arah dari kamera
        Vector3 dir = cameraTransform.forward;
        Vector3 origin;

        // 🔹 Jika mode First Person → ray dari kamera (mata)
        if (cameraController != null && cameraController.isFirstPerson)
        {
            origin = cameraTransform.position;
        }
        else
        {
            // 🔹 Jika mode Third Person → ray dari dada player
            origin = transform.position + Vector3.up * 1.3f + transform.forward * 0.5f;
        }

        // 1. Raycast utama
        if (Physics.Raycast(origin, dir, out hit, pickupRange))
            return hit;

        // 2. Spherecast kecil (cadangan)
        if (Physics.SphereCast(origin, 0.15f, dir, out hit, pickupRange))
            return hit;

        return null;
    }

    // ===== Highlight =====
    void UpdateHighlight()
    {
        if (RackInteract.UIActive)
        {
            InteractionPromptUI.Instance.Hide();
            return;
        }

        var hit = SmartRaycast();
        if (hit == null)
        {
            ClearHighlight();
            InteractionPromptUI.Instance.Hide();
            return;
        }

        RaycastHit h = hit.Value;
        var oh = h.collider.GetComponent<ObjectHighlight>();

        if (oh != null)
        {
            if (lastHighlight != null && lastHighlight != oh)
                lastHighlight.HighlightOff();

            lastHighlight = oh;
            oh.HighlightOn();
        }
        else
        {
            ClearHighlight();
        }

        string prompt = "";
        if (h.collider.GetComponent<IInteractable>() != null)
            prompt = "[E] / [Q] Interact";

        if (prompt != "")
            InteractionPromptUI.Instance.Show(prompt);
        else
            InteractionPromptUI.Instance.Hide();
    }

    void ClearHighlight()
    {
        if (lastHighlight != null)
            lastHighlight.HighlightOff();
        lastHighlight = null;
    }

    // ===== Input Hands =====
    void HandleHandsInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldRight == null) TryRightInteract();
            else DropRight();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (heldLeft == null) TryLeftInteract();
            else DropLeft();
        }
    }

    // ===== Right Hand =====
    void TryRightInteract()
    {
        var hit = SmartRaycast();
        if (hit == null) return;

        RaycastHit h = hit.Value;
        var interact = h.collider.GetComponent<IInteractable>();

        if (interact != null)
        {
            interact.OnInteractRight();
            return;
        }
    }

    void DropRight()
    {
        if (heldRight == null) return;

        heldRight.transform.SetParent(null);
        heldRight.useGravity = true;
        heldRight.isKinematic = false;
        heldRight = null;
    }

    // ===== Left Hand =====
    void TryLeftInteract()
    {
        var hit = SmartRaycast();
        if (hit == null) return;

        RaycastHit h = hit.Value;
        var interact = h.collider.GetComponent<IInteractable>();

        if (interact != null)
        {
            interact.OnInteractLeft();
            return;
        }
    }

    void DropLeft()
    {
        if (heldLeft == null) return;

        heldLeft.transform.SetParent(null);
        heldLeft.useGravity = true;
        heldLeft.isKinematic = false;
        heldLeft = null;
    }
}