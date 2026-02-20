using UnityEngine;

public class ButtonSequenceManager : MonoBehaviour
{
    [Header("Button Sequence Settings")]
    public DoorButton[] sequence; // Array of buttons in the sequence
    public DoorLock doorToUnock; // The door that will be unlocked when the sequence is completed

    private int currentIndex = 0; // Tracks the current button in the sequence

    public void ButtonPressed(DoorButton button)
    {
        if (button == sequence[currentIndex])
        {
            currentIndex++;
            if (currentIndex >= sequence.Length)
            {
                Debug.Log("Correct sequence! Unlocking door.");
                doorToUnock.OpenDoor();
                currentIndex = 0; // Reset the sequence
            }
        }
        else
        {
            Debug.Log("Wrong button! Resetting sequence.");
            currentIndex = 0; // Reset if the wrong button is pressed
        }
    }   
}
