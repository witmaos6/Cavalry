using UnityEngine;

public class EnemyRangeAttack : EnemyAttackBase
{
    public GameObject bulletPrefabs;

    public bool playAnimation = false;

    public override void Attack()
    {
        player = GetPlayerTarget();
        if (player != null)
        {
            if(!playAnimation)
            {
                Instantiate(bulletPrefabs, transform.position, transform.rotation);
            }
            else
            {
                Animator animator = enemy.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.CrossFade("RangeAttack", 0.1f);
                }
            }
        }
    }

    public void SpawnObject()
    {
        Vector3 dir = (player.position - transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle - 90f);
        
        Instantiate(bulletPrefabs, transform.position, rot);
    }
}
