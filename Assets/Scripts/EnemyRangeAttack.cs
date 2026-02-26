using UnityEngine;

public class EnemyRangeAttack : EnemyAttackBase
{
    public GameObject bulletPrefabs;
    private Transform player;

    public override void Attack()
    {
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            player = enemy.GetCurrentTarget();

            Instantiate(bulletPrefabs, transform.position, transform.rotation);
        }
    }
}
