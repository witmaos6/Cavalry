using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float normalSpeed = 10.0f;
    private Vector3 moveDir = Vector3.up;
    private bool playerTarget = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveDir = Vector3.up;
        Destroy(gameObject, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDir * normalSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(playerTarget && collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if(playerController != null)
            {
                playerController.TakeDamage(1);

                Destroy(gameObject);
            }
        }
        else if(collision.CompareTag("Shield"))
        {
            Destroy(gameObject);
        }
        else if(collision.CompareTag("Hwando"))
        {
            moveDir = -Vector3.up;
            playerTarget = false;
        }
        else if(!playerTarget && collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(5);

                Destroy(gameObject);
            }
        }
    }
}
