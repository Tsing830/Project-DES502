using System;
using UnityEngine;

public class EnemyRelocationOnSequenceComplete : MonoBehaviour
{
    [Serializable]
    public class EnemyRelocation
    {
        public EnemyController enemy;
        public Transform teleportTarget;
        public Transform[] newPatrolPoints;
        public bool startFromFirstPatrolPoint = true;
    }

    [Header("Sequence Source")]
    [SerializeField] private ButtonSequenceManager sequenceManager;
    [SerializeField] private bool triggerOnlyOnce = true;

    [Header("Enemy Relocation")]
    [SerializeField] private EnemyRelocation[] relocations;

    private bool hasTriggered;

    private void Awake()
    {
        ResolveSequenceManager();
    }

    private void OnEnable()
    {
        ResolveSequenceManager();

        if (sequenceManager != null)
            sequenceManager.OnSequenceCompleted += HandleSequenceCompleted;
    }

    private void OnDisable()
    {
        if (sequenceManager != null)
            sequenceManager.OnSequenceCompleted -= HandleSequenceCompleted;
    }

    private void HandleSequenceCompleted()
    {
        if (triggerOnlyOnce && hasTriggered)
            return;

        if (relocations == null || relocations.Length == 0)
        {
            Debug.LogWarning($"{name}: No enemy relocations are configured.");
            return;
        }

        for (int i = 0; i < relocations.Length; i++)
        {
            EnemyRelocation relocation = relocations[i];
            if (relocation == null || relocation.enemy == null)
                continue;

            relocation.enemy.TeleportAndSetPatrolRoute(
                relocation.teleportTarget,
                relocation.newPatrolPoints,
                relocation.startFromFirstPatrolPoint);
        }

        hasTriggered = true;
    }

    private void ResolveSequenceManager()
    {
        if (IsConfigured(sequenceManager))
            return;

        ButtonSequenceManager localManager = GetComponent<ButtonSequenceManager>();
        if (IsConfigured(localManager))
        {
            sequenceManager = localManager;
            return;
        }

        ButtonSequenceManager[] managers = FindObjectsByType<ButtonSequenceManager>(FindObjectsSortMode.None);
        for (int i = 0; i < managers.Length; i++)
        {
            if (!IsConfigured(managers[i]))
                continue;

            sequenceManager = managers[i];
            return;
        }
    }

    private static bool IsConfigured(ButtonSequenceManager manager)
    {
        return manager != null &&
               manager.sequence != null &&
               manager.sequence.Length > 0;
    }
}
