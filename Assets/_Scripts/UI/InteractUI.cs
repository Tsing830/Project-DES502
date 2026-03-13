using UnityEngine;

public class PlayerInteractUI : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject interactPrompt; // The "F" UI icon

    [Header("Raycast Settings")]
    public float interactRange = 3f;
    [Min(0f)] public float interactRadius = 0.2f;
    private Camera playerCam;

    void Start()
    {
        playerCam = Camera.main;

        // Hide prompt on start
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        CheckForInteractable();
    }

    void CheckForInteractable()
    {
        if (interactPrompt == null || playerCam == null) return;

        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // First try a precise center ray, then a small sphere cast for aim tolerance.
        if (TryGetInteractable(ray, out _))
        {
            if (!interactPrompt.activeSelf) interactPrompt.SetActive(true);
        }
        else
        {
            // Hide UI if looking at nothing
            if (interactPrompt.activeSelf) interactPrompt.SetActive(false);
        }
    }

    bool TryGetInteractable(Ray ray, out IInteractable interactable)
    {
        interactable = null;

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null) return true;
        }

        if (interactRadius <= 0f) return false;

        if (Physics.SphereCast(ray, interactRadius, out RaycastHit sphereHit, interactRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            interactable = sphereHit.collider.GetComponentInParent<IInteractable>();
            return interactable != null;
        }

        return false;
    }
}
