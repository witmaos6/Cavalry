using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum AttackType { Melee, Range } // To do: 삭제 예정

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

    private bool isDead = false;
    private bool canAttack = true;

    private EnemyAttackBase[] attackList;
    private EnemyUtilityBase[] utilityList;
    private float attackTotalRate = 0f;
    private bool runningUtility = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").transform;
        originHP = (int)hp;

        attackList = GetComponentsInChildren<EnemyAttackBase>();
        foreach (EnemyAttackBase attack in attackList)
        {
            attackTotalRate += attack.weight;
        }

        utilityList = GetComponentsInChildren<EnemyUtilityBase>();

        InvokeRepeating("UtilityCheck", 0.5f, 0.5f);
    }

    void UtilityCheck()
    {
        foreach (EnemyUtilityBase utility in utilityList)
        { 
            if(utility.ActivateCondition())
            {
                utility.Activate();
                runningUtility = true;
                // To do: EnemyUtilityBase에서 runningUtility 복원 시키는 기능 추가
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDead || player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        RotateToPlayer();

        if (distance > attackRange)
        {
            if(!runningUtility)
            {
                MoveToPlayer();
            }
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
        float randomAttackValue = Random.Range(0f, attackTotalRate);
        float currentSum = 0;

        foreach (EnemyAttackBase attack in attackList)
        {
            currentSum += attack.weight;
            if(randomAttackValue <= currentSum)
            {
                attack.Attack();
                break;
            }
        }

        StartCoroutine(AttackCooldown());
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

            CancelInvoke("UtilityCheck");
            Destroy(gameObject);
        }
    }

    public Transform GetCurrentTarget()
    {
        return player;
    }

    public void SetTargetTransform(Transform newTargetTransform)
    {
        player = newTargetTransform;
    }
}
