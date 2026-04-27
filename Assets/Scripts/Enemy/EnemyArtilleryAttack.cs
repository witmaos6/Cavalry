using System.Collections;
using UnityEngine;

public class EnemyArtilleryAttack : EnemyAttackBase
{
    public GameObject artilleryBullet;
    
    public float targetRandomRadius = 2.0f;
    public float explosionRadius = 1.5f;

    // 애니메이션 전용 멤버 변수
    public bool playAnimation = false;
    public GameObject warning;
    private GameObject warningInstance;
    public GameObject rock;

    public override void Attack()
    {
        player = GetPlayerTarget();
        if (!playAnimation)
        {
            Shot();
        }
        else
        {
            Animator animator = enemy.GetComponent<Animator>();
            if (animator != null)
            {
                animator.CrossFade("ArtilleryAttack", 0.1f);
            }
        }
    }

    void Shot() // ver. none animation 
    {
        if (player != null)
        {
            Vector2 randomOffset = Random.insideUnitCircle * targetRandomRadius;
            Vector3 targetPosition = player.position + (Vector3)randomOffset;

            Instantiate(artilleryBullet, targetPosition, Quaternion.identity);
        }
    }

// ver. play animation
    public void ReadyShot()
    {
        player = GetPlayerTarget();
        if (player != null)
        {
            Vector2 randomOffset = Random.insideUnitCircle * targetRandomRadius;
            Vector3 targetPosition = player.position + (Vector3)randomOffset;

            if(warningInstance != null)
            {
                Destroy(warningInstance);
            }

            warningInstance = Instantiate(warning, targetPosition, Quaternion.identity);

            warningInstance.transform.localScale = Vector3.one * explosionRadius * 2f;
            
            if(enemy != null)
            {
                enemy.dead += CompleteThrow;
            }
        }
    }

    public void ThrowRock()
    {
        if(warningInstance != null)
        {
            GameObject rockInstance = Instantiate(rock, transform.position, transform.rotation);
            EnemyArtilleryShotBullet enemyArtilleryShotBullet = rockInstance.GetComponent<EnemyArtilleryShotBullet>();
            if(enemyArtilleryShotBullet != null)
            {
                enemyArtilleryShotBullet.Init(this, transform.position, warningInstance.transform.position, explosionRadius);
            }
        }
    }

    public void CompleteThrow()
    {
        if(warningInstance != null)
        {
            Destroy(warningInstance);
        }
    }
}
