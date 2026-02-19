using JetBrains.Annotations;
using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [Header("Door Settings")]
    public string requiredKeyID; // The ID of the key required to unlock this door
    public bool isLocked = true; // Whether the door is currently locked
    public Vector3 openOffset = new Vector3(0, 0, 3f); // how far the door moves
    public float openSpeed = 3f;

    private bool isOpening = false;
    private Vector3 closedPos;
    private Vector3 openPos;


    private PlayerInventory playerInventory; // Reference to the player's inventory
    void Start()
    {
        playerInventory = FindFirstObjectByType<PlayerInventory>();

        closedPos = transform.position;
        openPos = closedPos + openOffset;

    }

    void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.Lerp(transform.position, openPos, Time.deltaTime * openSpeed);
        }
    }


    public void TryOpen()
    {
        if (!isLocked)
        {
            OpenDoor();
            return;
        }
        if (playerInventory.HasKey(requiredKeyID))
        {
            isLocked = false;
            OpenDoor();
        }
        else
        {
            Debug.Log("Door locked. Requires: " + requiredKeyID);
        }
    }

       public void OpenDoor()
        {
            Debug.Log("Door opened!");

            isOpening = true;


        }
    

        
}
