using UnityEngine;

public class EnemyArtilleryShotBullet : MonoBehaviour
{
    public float speed = 10.0f;
    public float arriveThreshold = 0.1f;
    public float explosionRadius = 1.5f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 direction;
    private bool isInitialized = false;

    private EnemyArtilleryAttack attackOwner;

    public void Init(EnemyArtilleryAttack inOwner, Vector3 start, Vector3 target, float inExplosionRadius)
    {
        attackOwner = inOwner;

        startPosition = start;
        targetPosition = target;

        transform.position = startPosition;
        direction = (targetPosition - startPosition).normalized;

        explosionRadius = inExplosionRadius;

        isInitialized = true;
    }
    
    void Update()
    {
        if (!isInitialized)
            return;

        Move();
    }

    void Move()
    {
        transform.position += direction * speed * Time.deltaTime;

        // µµÂø ÆÇÁ¤
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance <= arriveThreshold)
        {
            OnArrive();
        }
    }

    void OnArrive()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, explosionRadius, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            PlayerController pc = hit.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.TakeDamage(1);
            }
        }

        if(attackOwner != null)
        {
            attackOwner.CompleteThrow();
        }

        Destroy(gameObject);
    }
}
