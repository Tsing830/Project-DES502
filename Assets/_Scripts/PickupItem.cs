using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemName; 
    public float pickupRange = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            // Find the PickupItem on the hit object
            PickupItem item = hit.collider.GetComponentInParent<PickupItem>();

            // Only the item that was actually hit should run pickup logic
            if (item == null || item != this)
                return;

            PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
            if (inventory == null)
                return;

            // Check if this item has a KeyItem component
            KeyItem key = item.GetComponent<KeyItem>();

            if (key != null && !string.IsNullOrEmpty(key.keyID))
            {
                inventory.AddKey(key.keyID);
                Debug.Log("Picked up key: " + key.keyID);
            }
            else
            {
                inventory.AddItem(item.itemName);
                Debug.Log("Picked up item: " + item.itemName);
            }

            Destroy(gameObject);
        }

    }

}
