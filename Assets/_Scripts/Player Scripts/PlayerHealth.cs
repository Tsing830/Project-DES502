using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // The maximum health of the player
    public int currentHealth; // The current health of the player

    public System.Action<int, int> OnHealthChanged; // Event to notify when health changes

    private PlayerRespawn playerRespawn; // Reference to the PlayerRespawn component    

    void Start()
    {
        currentHealth = maxHealth; // Initialize current health to max health
        playerRespawn = FindFirstObjectByType<PlayerRespawn>(); // Get reference to PlayerRespawn component
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount; // Reduce current health by the damage amount
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player took damage: " + damageAmount + ". Current health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die(); // Call Die method if health drops to 0 or below
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount; // Increase current health by the heal amount
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player healed: " + healAmount + ". Current health: " + currentHealth);
    }

    public void Die() 
    { 
        Debug.Log("Player has died.");
        playerRespawn.Respawn();
        currentHealth = maxHealth;

    }
}
