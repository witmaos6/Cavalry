using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public float minSpeed = 5.0f;
    public float maxSpeed = 12.0f;
    public float acceleration = 2.0f;
    public float rotateSpeed = 300f;
    public float lifeTime = 5f;
    public float reflectionSpeed = 50.0f;
    // To do: 디테일한 수치 조정 필요

    private Rigidbody2D rb;
    private Transform player;
    private float currentSpeed;

    private bool playerTarget = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = minSpeed;
    }

    void Start()
    {
        player = GameObject.Find("Player").transform;
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (player == null || !playerTarget)
            return;

        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);

        Vector2 direction = (Vector2)player.position - rb.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.linearVelocity = transform.up * currentSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>()?.TakeDamage(1);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Shield"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Hwando"))
        {
            playerTarget = false;

            rb.AddForce(-transform.up * reflectionSpeed, ForceMode2D.Impulse);
        }
        else if (!playerTarget && collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(5);

                Destroy(gameObject);
            }
        }
    }
}
