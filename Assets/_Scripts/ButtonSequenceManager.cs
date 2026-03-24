using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSequenceManager : MonoBehaviour
{
    public event Action OnSequenceCompleted;

    [Header("Button Sequence Settings")]
    public DoorButton[] sequence; // Array of buttons in the sequence
    public DoorLock[] doorsToUnlock; // The door that will be unlocked when the sequence is completed

    [Header("Objective Update (Optional)")]
    [SerializeField] private Image objectiveImage;
    [SerializeField] private Sprite objectiveAfterSequenceComplete;
    [SerializeField] private string objectiveImageObjectName = "Objective";
    [SerializeField] private bool updateObjectiveOnSequenceComplete = true;

    private int currentIndex = 0; // Tracks the current button in the sequence
    private bool objectiveUpdated;

    private void Awake()
    {
        if (string.IsNullOrEmpty(objectiveImageObjectName))
            objectiveImageObjectName = "Objective";

        ResolveObjectiveImageReference();
    }

    public void ButtonPressed(DoorButton button)
    {
        if (sequence == null || sequence.Length == 0)
        {
            Debug.LogWarning($"{name}: Button sequence is not configured.");
            return;
        }

        if (button == sequence[currentIndex])
        {
            currentIndex++;
            if (currentIndex >= sequence.Length)
            {
                Debug.Log("Correct sequence! Unlocking door.");
                if (doorsToUnlock != null)
                {
                    foreach (DoorLock doorToUnlock in doorsToUnlock)
                        doorToUnlock.OpenDoor();
                }

                UpdateObjectiveAfterSequenceComplete();
                OnSequenceCompleted?.Invoke();
                currentIndex = 0; // Reset the sequence
            }
        }
        else
        {
            Debug.Log("Wrong button! Resetting sequence.");
            currentIndex = 0; // Reset if the wrong button is pressed
        }
    }

    private void UpdateObjectiveAfterSequenceComplete()
    {
        if (!updateObjectiveOnSequenceComplete || objectiveUpdated)
            return;

        if (objectiveAfterSequenceComplete == null)
        {
            Debug.LogWarning($"{name}: objectiveAfterSequenceComplete is not assigned.");
            return;
        }

        ResolveObjectiveImageReference();
        if (objectiveImage == null)
        {
            Debug.LogWarning($"{name}: Objective image not found.");
            return;
        }

        objectiveImage.sprite = objectiveAfterSequenceComplete;
        objectiveUpdated = true;
        Debug.Log("Objective updated to: Get to the warehouse.");
    }

    private void ResolveObjectiveImageReference()
    {
        if (objectiveImage != null || string.IsNullOrEmpty(objectiveImageObjectName))
            return;

        GameObject objectiveObject = GameObject.Find(objectiveImageObjectName);
        if (objectiveObject != null)
            objectiveImage = objectiveObject.GetComponent<Image>();
    }
}
