using UnityEngine;

public abstract class EnemyAttackBase : MonoBehaviour
{
    public float weight = 1f;
    public float damage = 1f;
    protected Transform player;

    public abstract void Attack();

    public Transform GetPlayerTarget()
    {
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            return enemy.GetCurrentTarget();
        }
        return null;
    }
}
