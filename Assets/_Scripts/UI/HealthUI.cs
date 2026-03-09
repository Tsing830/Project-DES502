using UnityEngine;
using UnityEngine.UI;


public class HealthUI : MonoBehaviour
{
    public Image healthImage;
    public Sprite[] healthSprites; // 0 = dead, 6 = full

    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("HealthUI could not find PlayerHealth in scene.");
            return;
        }

        playerHealth.OnHealthChanged += UpdateHealthUI;

        // Initialize UI
        UpdateHealthUI(playerHealth.currentHealth, playerHealth.maxHealth);
    }
    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    void UpdateHealthUI(int current, int max)
    {
        float percent = (float)current / max * 100f;

        if (percent >= 100f)
            healthImage.sprite = healthSprites[6];
        else if (percent >= 83f)
            healthImage.sprite = healthSprites[5];
        else if (percent >= 66f)
            healthImage.sprite = healthSprites[4];
        else if (percent >= 50f)
            healthImage.sprite = healthSprites[3];
        else if (percent >= 33f)
            healthImage.sprite = healthSprites[2];
        else if (percent >= 16f)
            healthImage.sprite = healthSprites[1];
        else
            healthImage.sprite = healthSprites[0];
    }

}
