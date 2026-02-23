using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float normalSpeed = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * normalSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if(playerController != null)
            {
                playerController.TakeDamage(1);

                Destroy(gameObject);
            }
        }
    }
}
