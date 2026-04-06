using System.Collections;
using UnityEngine;

public abstract class EnemyAttackBase : MonoBehaviour
{
    [Header("Attack Base Settings")]
    public float weight = 1f;
    public float damage = 1f;
    public float attackDistance = 2f;
    public string attackStateName = "Attack";

    protected Transform player;
    protected Enemy enemy;

    public abstract void Attack();

    public Transform GetPlayerTarget()
    {
        enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            return enemy.GetCurrentTarget();
        }
        return null;
    }

    protected void StartAttack(float attackTime)
    {
        StartCoroutine(StartAttacking(attackTime));
    }

    IEnumerator StartAttacking(float attackTime)
    {
        if(enemy != null)
        {
            enemy.SetAttacking(true);
        }
        yield return new WaitForSeconds(attackTime);
        if(enemy != null)
        {
            enemy.SetAttacking(false);
        }
    }
}
