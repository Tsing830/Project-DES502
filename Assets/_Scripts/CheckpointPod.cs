using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour, IInteractable
{
    [Header("Checkpoint Settings")]
    public Transform checkpointLocation; // The location where the player will respawn
    public int healAmpount = 100;
    public EnemyManager enemyManager; // Reference to the enemy controller to reset enemies

    public void Interact() 
    {
        Debug.Log("Checkpoint activated");

        // Heal the player to full health
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.Heal(healAmpount);

        // Set the player's respawn point to the checkpoint location
        PlayerRespawn playerRespawn = FindFirstObjectByType<PlayerRespawn>();
        if (playerRespawn != null)
            playerRespawn.SetRespawnPoint(checkpointLocation.position + checkpointLocation.forward * 1.5f );

        // Reset enemies in the scene
        if (enemyManager != null)
            enemyManager.ResetEnemies();

    }

}
