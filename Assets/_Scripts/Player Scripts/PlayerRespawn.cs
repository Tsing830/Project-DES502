using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 respawnPoint; // The point where the player will respawn

    void Start()
    {
        respawnPoint = transform.position; // Set initial respawn point to the player's starting position
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint; // Update the respawn point
        Debug.Log("Respawn point set to: " + respawnPoint);
    }

    public void Respawn()
    {
        transform.position = respawnPoint; // Move the player to the respawn point
        Debug.Log("Player respawned at: " + respawnPoint);
    }
}
