using System.Collections;
using UnityEngine;

public class EnemyArtilleryAttack : EnemyAttackBase
{
    public GameObject warningPrefab;
    public GameObject explosionPrefab; // To do: 추후에 파티클로 변경
    public float targetRandomRadius = 2.0f;
    public float delayTime = 1.0f;
    public float explosionRadius = 1.5f;

    public override void Attack()
    {
        player = GetPlayerTarget();
        if(player != null)
        {
            Vector2 randomOffset = Random.insideUnitCircle * targetRandomRadius;
            Vector3 targetPosition = player.position + (Vector3)randomOffset;

            StartCoroutine(ArtillerySequence(targetPosition));
        }
    }

    private IEnumerator ArtillerySequence(Vector3 position)
    {
        GameObject warning = null;
        if (warningPrefab != null)
        {
            warning = Instantiate(warningPrefab, position, Quaternion.identity);
            warning.transform.localScale = Vector3.one * explosionRadius * 2f;
        }

        yield return new WaitForSeconds(delayTime);

        if (warning != null)
        {
            Destroy(warning);
        }

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * explosionRadius * 2f;

            Collider2D hit = Physics2D.OverlapCircle(position, explosionRadius, LayerMask.GetMask("Player"));
            if (hit != null)
            {
                PlayerController pc = hit.GetComponent<PlayerController>();
                if (pc != null)
                {
                    pc.TakeDamage(1);
                }
            }
            StartCoroutine(DestroyExplosion(explosion));
        }
    }

    IEnumerator DestroyExplosion(GameObject explosion) // To do: explosionPrefab을 파티클로 변경 시 삭제해도 될 수 있음
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(explosion);
    }
}
