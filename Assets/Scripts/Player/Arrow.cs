using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float normalSpeed = 50.0f;
    public float baseLifeTime = 1.5f;

    private Rigidbody2D rb;
    
    private bool isFullCharge = false;

    public GameObject owner;
    private PlayerController playerController;
    private float ownerAttackPower = 0f;
    private int hitCount = 0;

    public GameObject hitParticlePrefab;
    public GameObject blockParticlePrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        playerController = owner.GetComponent<PlayerController>();
        if(playerController != null)
        {
            if(playerController.IsUnlockSkill(GameData.SkillID.BigArrow))
            {
                transform.localScale = transform.localScale * 2f;
            }
            ownerAttackPower = playerController.attackPower;
        }
    }

    public void Setup(bool fullCharge, float powerMultiplier, GameObject inOwner)
    {
        owner = inOwner;

        isFullCharge = fullCharge;

        float finalSpeed = normalSpeed * powerMultiplier;
        rb.AddForce(transform.up * finalSpeed, ForceMode2D.Impulse);

        if(gameObject != null)
        {
            float finalLifeTime = baseLifeTime * powerMultiplier;
            Destroy(gameObject, finalLifeTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            SpawnHitParticle(collision.transform.position);

            if(playerController != null)
            {
                if(playerController.IsUnlockSkill(GameData.SkillID.OnemoreTimeShot))
                {
                    playerController.ResetAttackCoolDown();
                }
            }

            Destroy(collision.gameObject);

            if (!isFullCharge)
            {
                Destroy(gameObject);
            }
        }
        else if(collision.CompareTag("Enemy"))
        {
            SpawnHitParticle(collision.transform.position);

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if(enemy != null)
            {
                float damage = ownerAttackPower;
                if(isFullCharge)
                {
                    damage *= 2.5f;
                }
                enemy.TakeDamage(damage);
            }

            if(playerController.IsUnlockSkill(GameData.SkillID.MultiKill))
            {
                hitCount++;

                if(hitCount >= 3)
                {
                    playerController.IncreaseAttackPowerBuff(5f, 2f);
                    playerController.ResetAttackCoolDown();
                }
            }

            if(!isFullCharge)
            {
                Destroy(gameObject);
            }
        }
        else if(collision.CompareTag("EnemyShield"))
        {
            if (!isFullCharge)
            {
                SpawnBlockParticle(collision.transform.position);
                Destroy(gameObject);
            }
        }
    }

    void SpawnHitParticle(Vector3 collisionPosition)
    {
        Vector3 spawnPosition = new Vector3(collisionPosition.x, collisionPosition.y, collisionPosition.z - 1.0f);
        GameObject hitParticleInstance = Instantiate(hitParticlePrefab, spawnPosition, Quaternion.identity);
        if (hitParticleInstance != null)
        {
            Destroy(hitParticleInstance, 0.5f);
        }
    }

    void SpawnBlockParticle(Vector3 collisionPosition)
    {
        Vector3 spawnPosition = new Vector3(collisionPosition.x, collisionPosition.y, collisionPosition.z - 1.0f);
        GameObject blockParticleInstance = Instantiate(blockParticlePrefab, spawnPosition, Quaternion.identity);
        if (blockParticleInstance != null)
        {
            Destroy(blockParticleInstance, 0.5f);
        }
    }
}
