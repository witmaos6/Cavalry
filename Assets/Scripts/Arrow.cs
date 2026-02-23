using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float normalSpeed = 50.0f;

    private Rigidbody2D rb;
    
    private bool isFullCharge = false;

    public float baseLifeTime = 1.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    public void Setup(bool fullCharge, float powerMultiplier)
    {
        isFullCharge = fullCharge;

        float finalSpeed = normalSpeed * powerMultiplier;
        rb.AddForce(transform.up * finalSpeed, ForceMode2D.Impulse);

        if(gameObject != null)
        {
            float finalLifeTime = baseLifeTime * powerMultiplier;
            Destroy(gameObject, finalLifeTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            Destroy(collision.gameObject);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(30);
            }

            if (!isFullCharge)
            {
                Destroy(gameObject);   
            }
        }
        else if(collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if(enemy != null)
            {
                float damage = isFullCharge ? 5 : 2;
                enemy.TakeDamage(damage);
            }

            if(!isFullCharge)
            {
                Destroy(gameObject);
            }
        }
    }
}
