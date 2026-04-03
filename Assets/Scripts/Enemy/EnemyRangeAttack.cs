using UnityEngine;

public class EnemyRangeAttack : EnemyAttackBase
{
    public GameObject bulletPrefabs;

    public override void Attack()
    {
        player = GetPlayerTarget();
        if (player != null)
        {
            Instantiate(bulletPrefabs, transform.position, transform.rotation);
        }
    }
}
