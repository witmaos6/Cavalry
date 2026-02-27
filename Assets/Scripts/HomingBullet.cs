using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotateSpeed = 200f;
    public float lifeTime = 3f;
    // To do: 디테일한 수치 조정 필요

    private Rigidbody2D rb;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 0.5f;
    }

    void Start()
    {
        player = GameObject.Find("Player").transform;
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 direction = (Vector2)player.position - rb.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.AddForce(transform.up * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>()?.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}
