using UnityEngine;

public class DoorButton : MonoBehaviour, IInteractable
{
    public DoorLock linkedDoor; // Reference to the door this button controls
    public ButtonSequenceManager sequenceManager; // Optional reference to a sequence manager if this button is part of a sequence

    public void Interact()
    {
        Debug.Log("Button pressed: " + name);

        if (sequenceManager != null)
        {
            sequenceManager.ButtonPressed(this);
            return; // If part of a sequence, let the manager handle the logic
        }   


        if (linkedDoor != null)
        {
            linkedDoor.OpenDoor();
            Debug.Log("Button pressed");
        }
    }
}
