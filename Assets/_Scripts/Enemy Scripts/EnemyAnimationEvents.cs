using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private EnemyController enemyController;
    [SerializeField] private AK.Wwise.Event footstepEvent;

    void Start()
    {
        // Searches the parent hierarchy for EnemyController
        enemyController = GetComponentInParent<EnemyController>();
    }

    public void PlayFootstepSound()
    {
        if (footstepEvent != null && footstepEvent.IsValid())
        {
            footstepEvent.Post(gameObject);
        }
    }
}
