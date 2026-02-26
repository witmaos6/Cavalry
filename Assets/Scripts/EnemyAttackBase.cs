using UnityEngine;

public abstract class EnemyAttackBase : MonoBehaviour
{
    public float weight = 1f;
    public float damage = 1f;

    public abstract void Attack();
}
