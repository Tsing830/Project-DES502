using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationEventFootstepReceiver : MonoBehaviour
{
    public void PlayFootstepSound()
    {
        var footSteps = GetComponent<FootSteps>() ?? GetComponentInParent<FootSteps>();
        if (footSteps != null)
        {
            footSteps.PlayFootstepSound();
            return;
        }

        var enemyEvents = GetComponent<EnemyAnimationEvents>() ?? GetComponentInParent<EnemyAnimationEvents>();
        if (enemyEvents != null)
        {
            enemyEvents.PlayFootstepSound();
        }
    }
}

public static class AnimationEventFootstepReceiverBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded += (_, __) => EnsureReceivers();
        EnsureReceivers();
    }

    private static void EnsureReceivers()
    {
#if UNITY_6000_0_OR_NEWER
        var animators = Object.FindObjectsByType<Animator>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
        var animators = Object.FindObjectsOfType<Animator>(true);
#endif
        foreach (var animator in animators)
        {
            if (animator == null)
            {
                continue;
            }

            if (animator.GetComponent<AnimationEventFootstepReceiver>() == null)
            {
                animator.gameObject.AddComponent<AnimationEventFootstepReceiver>();
            }
        }
    }
}
