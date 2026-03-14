using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyArtilleryBullet : MonoBehaviour
{
    public GameObject warningPrefab;
    public GameObject explosionPrefab; // To do: 추후에 파티클로 변경
    public float delayTime = 1.0f;
    public float explosionRadius = 1.5f;

    private GameObject warning;

    void Start()
    {
        if (warningPrefab != null)
        {
            warning = Instantiate(warningPrefab, transform.position, Quaternion.identity);
            warning.transform.localScale = Vector3.one * explosionRadius * 2f;

            StartCoroutine(ArtillerySequence());
        }
    }
 
    IEnumerator ArtillerySequence()
    {
        yield return new WaitForSeconds(delayTime);

        if (warning != null)
        {
            Destroy(warning);
        }

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * explosionRadius * 2f;

            Collider2D hit = Physics2D.OverlapCircle(transform.position, explosionRadius, LayerMask.GetMask("Player"));
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
        Destroy(gameObject);
    }
}
