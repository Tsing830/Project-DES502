using UnityEngine;

public class EnemyManager : MonoBehaviour
{
   public EnemyController[] enemies; // Array of all enemy controllers in the scene
    public void ResetEnemies()
    {
        foreach (EnemyController enemy in enemies)
        {
            enemy.ResetState();
        }

        Debug.Log("Enemies reset!");

    }
}
