using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public DoorLock linkedDoor; // Reference to the door this button controls
    public bool isPressed = false; // Whether the button is currently pressed

    public void PressButton()
    {
        if (linkedDoor != null)
        {
            linkedDoor.OpenDoor();
            isPressed = true;
            Debug.Log("Button pressed!");
        }
        else
        {
            Debug.LogWarning("No door linked to this button!");
        }
    }
}
