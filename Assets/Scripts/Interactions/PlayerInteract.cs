using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public float interactRange = 3f;
    public LayerMask interactableLayer;

    private IInteractable currentTarget;
    private RaycastHit hit;

    void Update()
    {
        DetectInteractable();
        HandleInput();
    }

    void DetectInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            var interact = hit.collider.GetComponent<IInteractable>();

            if (interact != null)
            {
                currentTarget = interact;

                string promptText = interact.GetPromptMessage();
                InteractionPromptUI.Instance.Show(promptText);
                return;
            }
        }

        currentTarget = null;
        InteractionPromptUI.Instance.Hide();
    }

    void HandleInput()
    {
        if (currentTarget == null) return;

        if (Input.GetKeyDown(KeyCode.E))
            currentTarget.OnInteractRight();
        else if (Input.GetKeyDown(KeyCode.Q))
            currentTarget.OnInteractLeft();
    }

    void OnDrawGizmos()
    {
        if (playerCamera == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactRange);
    }
}