using UnityEngine;

public class StealthUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject stealthIndicator;

    [Header("Detection Rules")]
    [SerializeField] private bool treatSuspectAsDetected = true;
    [SerializeField] private EnemyController[] enemies;
    [SerializeField] private float enemyRefreshInterval = 1f;

    private SimplePlayerController playerController;
    private CanvasGroup selfCanvasGroup;
    private float refreshTimer;

    void Awake()
    {
        if (stealthIndicator == null)
            stealthIndicator = gameObject;

        if (stealthIndicator == gameObject)
        {
            selfCanvasGroup = GetComponent<CanvasGroup>();
            if (selfCanvasGroup == null)
                selfCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        playerController = FindFirstObjectByType<SimplePlayerController>();
        RefreshEnemies();
        UpdateStealthUI();
    }

    void Update()
    {
        if (playerController == null)
            playerController = FindFirstObjectByType<SimplePlayerController>();

        refreshTimer += Time.deltaTime;
        if (refreshTimer >= enemyRefreshInterval)
        {
            RefreshEnemies();
            refreshTimer = 0f;
        }

        UpdateStealthUI();
    }

    private void RefreshEnemies()
    {
        if (enemies == null || enemies.Length == 0)
            enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
    }

    private void UpdateStealthUI()
    {
        bool isSneaking = playerController != null &&
                          playerController.CurrentState == SimplePlayerController.PlayerState.Sneaking;

        bool isDetected = IsPlayerDetected();
        bool shouldShow = isSneaking && !isDetected;

        if (stealthIndicator == null)
            return;

        if (stealthIndicator == gameObject && selfCanvasGroup != null)
        {
            selfCanvasGroup.alpha = shouldShow ? 1f : 0f;
            selfCanvasGroup.interactable = false;
            selfCanvasGroup.blocksRaycasts = false;
            return;
        }

        if (stealthIndicator.activeSelf != shouldShow)
            stealthIndicator.SetActive(shouldShow);
    }

    private bool IsPlayerDetected()
    {
        if (enemies == null || enemies.Length == 0)
            return false;

        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyController enemy = enemies[i];
            if (enemy == null)
                continue;

            if (enemy.currentState == EnemyController.EnemyState.Chase)
                return true;

            if (treatSuspectAsDetected && enemy.currentState == EnemyController.EnemyState.Suspect)
                return true;
        }

        return false;
    }
}