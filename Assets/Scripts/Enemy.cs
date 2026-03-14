using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float hp = 10;
    private int originHP;
    public float moveSpeed = 3.0f;
    public float attackRate = 1.0f;
    public float damage = 1.0f;

    private Rigidbody2D rb;

    private Transform player;

    private bool isDead = false;
    private bool canAttack = true;

    private EnemyAttackBase[] attackList;
    private List<EnemyUtilityBase> sensorUtility = new List<EnemyUtilityBase>();
    private float attackTotalRate = 0f;
    private bool runningUtility = false;

    public delegate void Dead();
    public Dead dead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        player = GameObject.Find("Player").transform;
        originHP = (int)hp;

        attackList = GetComponentsInChildren<EnemyAttackBase>();
        foreach (EnemyAttackBase attack in attackList)
        {
            attackTotalRate += attack.weight;
        }

        EnemyUtilityBase[] utilityList = GetComponentsInChildren<EnemyUtilityBase>();
        foreach(EnemyUtilityBase utility in utilityList)
        {
            if(utility.utilityType == EnemyUtilityBase.UtilityType.Sensor)
            {
                sensorUtility.Add(utility);
            }
        }

        InvokeRepeating("UtilityCheck", 0.5f, 0.5f);
    }

    void UtilityCheck()
    {
        foreach (EnemyUtilityBase utility in sensorUtility)
        { 
            if(utility.ActivateCondition())
            {
                utility.Activate();
                StartCoroutine(ResetUtility());
                break;
            }
        }
    }

    public void CalledRunningUtility()
    {
        StartCoroutine(ResetUtility());
    }

    IEnumerator ResetUtility()
    {
        runningUtility = true;
        yield return new WaitForSeconds(0.3f);
        runningUtility = false;
    }

    private void FixedUpdate()
    {
        if (isDead || player == null)
            return;        

        RotateToPlayer();

        if (!runningUtility)
        {
            MoveToPlayer();
        }
        
        if(canAttack)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 0.1f);

            Attack();
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
        float distance = Vector2.Distance(transform.position, player.position);

        float randomAttackValue = Random.Range(0f, attackTotalRate);
        float currentSum = 0;

        foreach (EnemyAttackBase attack in attackList)
        {
            currentSum += attack.weight;
            if(randomAttackValue <= currentSum && distance <= attack.attackDistance)
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

            CancelInvoke("UtilityCheck");
            if(dead != null)
            {
                dead.Invoke();
                dead = null;
            }
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
