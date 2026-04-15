using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyScatterAttack : EnemyAttackBase
{
    public GameObject bulletPrefab;
    public int bulletCount = 2;
    public float spreadAngle = 45f;

    public bool playAnimation = false;

    public override void Attack()
    {
        player = GetPlayerTarget();

        if (!playAnimation)
        {
            SpwanObject();
        }
        else
        {
            Animator animator = enemy.GetComponent<Animator>();
            if (animator != null)
            {
                animator.CrossFade("Attack", 0.1f);
            }
        }
    }

    public void SpwanObject()
    {
        if (player != null)
        {
            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            float centerAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;

            float startAngle = centerAngle - (spreadAngle / 2f);

            float angleStep = spreadAngle / (bulletCount - 1);

            for (int i = 0; i < bulletCount; i++)
            {
                float currentAngle = startAngle + (angleStep * i);

                Quaternion rotation = Quaternion.Euler(0, 0, currentAngle - 90f);

                Instantiate(bulletPrefab, transform.position, rotation);
            }
        }
    }
}
