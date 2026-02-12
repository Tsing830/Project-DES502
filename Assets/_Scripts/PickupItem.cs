using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemName; 

    public float pickupRange = 2f;

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
            if (hit.collider.GetComponentInParent<PickupItem>() == this)

            {
                PlayerInventory inventory = FindFirstObjectByType<PlayerInventory>();
                if (inventory != null)
                {
                    inventory.AddItem(itemName);
                    Destroy(gameObject);
                }
            }
        }
    }
}
