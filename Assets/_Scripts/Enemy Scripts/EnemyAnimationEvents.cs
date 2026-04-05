using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private EnemyController enemyController;

    void Start()
    {
        // Searches the parent hierarchy for EnemyController
        enemyController = GetComponentInParent<EnemyController>();
    }

    public void PlayFootstepSound()
    {
        AkSoundEngine.PostEvent("Play_WardenFootsteps", gameObject);
    }
}
