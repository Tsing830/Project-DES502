using UnityEngine;
using static UnityEditor.Progress;

public class PickupItem : MonoBehaviour, IInteractable
{
    public string itemName; 
    
    public void Interact()
    {
        PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();
    

        KeyItem key = GetComponent<KeyItem>();

        if (key != null && !string.IsNullOrEmpty(key.keyID))
        {
            playerInventory.AddKey(key.keyID);
            Debug.Log("Picked up key: " + key.keyID);
        }
        else
        {
            playerInventory.AddItem(itemName);
            Debug.Log("Picked up item: " + itemName);
        }
        Destroy(gameObject);
    }


}
