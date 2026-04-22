using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : EnemyAttackBase
{
    // temp logic
    // To do: 추후에 완전히 애니메이션 재생 로직으로 바꿀 때 수정 필요
    public Transform weaponTransform;
    public float stabDistance = 0.5f;
    public float stabSpeed = 10f;
    public bool playAnimation = false;

    public override void Attack()
    {
        enemy = GetComponent<Enemy>();
        player = GetPlayerTarget();
        if (enemy != null && player != null)
        {
            if(!playAnimation)
            {
                StartCoroutine(StabRoutine());
                StartAttack(1.0f);
            }
            else
            {
                Animator animator = enemy.GetComponent<Animator>();
                if(animator != null )
                {
                    animator.CrossFade("MeleeAttack", 0.1f);
                }
            }
        }
    }

    IEnumerator StabRoutine()
    {
        Vector3 originalPos = weaponTransform.localPosition;
        Vector3 targetPos = originalPos + Vector3.up * stabDistance;

        float t = 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime * stabSpeed;
            weaponTransform.localPosition = Vector3.Lerp(originalPos, targetPos, t);
            yield return null;
        }

        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= attackDistance + stabDistance)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(1);
                }
            }
        }

        t = 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime * stabSpeed * 0.5f;
            weaponTransform.localPosition = Vector3.Lerp(targetPos, originalPos, t);
            yield return null;
        }

        weaponTransform.localPosition = originalPos;
    }

    public void HitAttack()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= attackDistance + stabDistance)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(1);
                }
            }
        }
    }
}
