using UnityEngine;

public class PlayerInteractUI : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject interactPrompt; // The "F" UI icon

    [Header("Raycast Settings")]
    public float interactRange = 3f;
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

        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);

        // Check if looking at an object within range
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                // Show UI if it's interactable
                if (!interactPrompt.activeSelf) interactPrompt.SetActive(true);
            }
            else
            {
                // Hide UI if it's not interactable
                if (interactPrompt.activeSelf) interactPrompt.SetActive(false);
            }
        }
        else
        {
            // Hide UI if looking at nothing
            if (interactPrompt.activeSelf) interactPrompt.SetActive(false);
        }
    }
}