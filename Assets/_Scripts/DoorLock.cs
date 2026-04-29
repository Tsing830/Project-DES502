using JetBrains.Annotations;
using UnityEngine;

public class DoorLock : MonoBehaviour, IInteractable    
{
    [Header("Door Settings")]
    public string requiredKeyID; // The ID of the key required to unlock this door
    public bool isLocked = true; // Whether the door is currently locked
    public Vector3 openOffset = new Vector3(0, 0, 3f); // how far the door moves
    public float openSpeed = 3f;
    public Animator animator;

    [Header("Warden Trigger (e.g. Server Room Door)")]
    [Tooltip("If set, unlocking this door with a key permanently locks the Warden into chase mode.")]
    public EnemyController wardenToEngage;

    private bool isOpening = false;
    private Vector3 closedPos;
    private Vector3 openPos;

    public void Interact()
    {
        TryOpen();
    }

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
            animator.Play("DoorOpen");
            //transform.position = Vector3.Lerp(transform.position, openPos, Time.deltaTime * openSpeed);
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
            if (wardenToEngage != null)
            {
                wardenToEngage.StartPermanentChase();
            }
        }
        else
        {
            Debug.Log("Door locked. Requires: " + requiredKeyID);
        }
    }

       public void OpenDoor()
        {
            if (isOpening) return;  // Stops DoorOpen sound from playing repeatedly - Charlotte
            Debug.Log("Door opened!");
            AkUnitySoundEngine.PostEvent("Play_DoorOpen", gameObject); // Plays DoorOpen sound - Charlotte
            isOpening = true;
        }
    

        
}
