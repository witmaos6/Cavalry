using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : EnemyAttackBase
{
    public Transform weaponTransform;
    public float stabDistance = 0.5f;
    public float stabSpeed = 10f;

    public override void Attack()
    {
        Enemy enemy = GetComponent<Enemy>();
        player = GetPlayerTarget();
        if (enemy != null && player != null)
        {
            StartCoroutine(StabRoutine());
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
}
