using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum AttackType { Melee, Range }

    [Header("Enemy Settings")]
    public AttackType type = AttackType.Melee;
    public float hp = 10;
    private int originHP;
    public float moveSpeed = 3.0f;
    public float attackRange = 1.5f;
    public float attackRate = 1.0f;
    public float damage = 1.0f;

    private Rigidbody2D rb;

    private Transform player;

    [Header("Range Enemy Settings")]
    public GameObject bulletPrefabs;

    [Header("Melee Enemy Settings")]
    public Transform weaponTransform;
    public float stabDistance = 0.5f;
    public float stabSpeed = 10f;

    private bool isDead = false;
    private bool canAttack = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").transform;
        originHP = (int)hp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (isDead || player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        RotateToPlayer();

        if (distance > attackRange)
        {
            MoveToPlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 0.1f);

            if (canAttack)
            {
                Attack();
            }
        }
    }

    void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    void RotateToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void Attack()
    {
        if(type == AttackType.Melee)
        {
            if(weaponTransform != null)
            {
                StartCoroutine(StabRoutine());
            }
        }
        else if(type == AttackType.Range)
        {
            Instantiate(bulletPrefabs, transform.position, transform.rotation);
        }

        StartCoroutine(AttackCooldown());
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

        if(player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if(distance <= attackRange + stabDistance)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if(playerController != null)
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

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackRate);
        canAttack = true;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0 && !isDead)
        {
            isDead = true;

            if(GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(originHP);
            }

            Destroy(gameObject);
        }
    }
}
