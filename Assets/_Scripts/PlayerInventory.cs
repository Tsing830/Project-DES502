using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int maxItems = 10; // Maximum number of items the player can carry
    public List<string> inventory = new List<string>(); // List to hold the player's items

    public void AddItem(string item)
    {
        if (inventory.Count < maxItems)
        {
            inventory.Add(item);
            Debug.Log($"Added {item} to inventory. Total items: {inventory.Count}");
        }
        else
        {
            Debug.Log("Inventory is full! Cannot add more items.");
        }
    }

    public void RemoveItem(string item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            Debug.Log($"Removed {item} from inventory. Total items: {inventory.Count}");
        }
        else
        {
            Debug.Log($"Item {item} not found in inventory.");
        }
    }

    public void ListInventory()
    {
        Debug.Log("Current Inventory:");
        foreach (string item in inventory)
        {
            Debug.Log(item);
        }
    }

    public bool HasItem(string item)
    {
        return inventory.Contains(item);
    }
}
