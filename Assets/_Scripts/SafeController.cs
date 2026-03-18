using UnityEngine;
using System.Collections;

public class SafeController : MonoBehaviour, IInteractable
{
    public string correctCode = "1234"; // The correct code to open the safe
    public bool hasBeenOpened = false;

    private SafeUIController safeUI; // Reference to the SafeUI component 

    void Start()
    {
        safeUI = Object.FindFirstObjectByType<SafeUIController>(FindObjectsInactive.Include); // Find the SafeUIController in the scene
    }

    public void Interact()
    {
        if (hasBeenOpened)
        {
            Debug.Log("Safe already opened.");
        } else if (safeUI != null)
        {
            safeUI.OpenSafeUI(this); // Open the safe UI when the player interacts with the safe
        }
        else
        {
            Debug.LogError("SafeUIController not found in the scene.");
        }
    }

    public bool CheckCode(string enteredCode)
    {
        if (enteredCode == correctCode)
        {
            Debug.Log("Safe opened! Correct code entered.");

            hasBeenOpened = true; // Mark safe as permanently opened
            GiveReward();         // Give the keycard


            StartCoroutine(CloseAfterDelay());
            return true;
        }
        else
        {
            Debug.Log("Incorrect code. Try again.");
            return false;
        }

    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2f);
        safeUI.CloseSafeUI();
    }

    private void GiveReward()
    {
        PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddKey("ServerRoomKeycard");
            Debug.Log("Player received keycard.");
        }
    }



}
