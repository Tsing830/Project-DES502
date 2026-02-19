using UnityEngine;

public class DoorButton : MonoBehaviour, IInteractable
{
    public DoorLock linkedDoor; // Reference to the door this button controls
 
    public void Interact()
    {
        if (linkedDoor != null)
        {
            linkedDoor.OpenDoor();
            Debug.Log("Button pressed");


        }
    }
}
