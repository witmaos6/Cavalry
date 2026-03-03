using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public float lifeTime = 1.0f;
    public float skillCoolTime = 1.0f;
    public float radius = 5.0f;
    private List<Enemy> enemies = new List<Enemy>();
    private GameObject owner;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LifeTimeout());

        InvokeRepeating("Skill", 0.0f, skillCoolTime);
    }

    void Update()
    {
        
    }

    public void SetOwner(GameObject inOwner)
    {
        owner = inOwner;
    }

    IEnumerator LifeTimeout()
    {
        yield return new WaitForSeconds(lifeTime);

        foreach(Enemy enemy in enemies)
        {
            if(enemy != null)
            {
                enemy.SetTargetTransform(owner.transform);
            }
        }

        Destroy(gameObject);
    }

    void Skill()
    {
        int enemyMask = LayerMask.GetMask("Enemy");

        Collider2D[] scan = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);

        foreach(Collider2D enemy in scan)
        {
            Enemy enemyComp = enemy.GetComponent<Enemy>();
            if(enemyComp != null)
            {
                enemies.Add(enemyComp);
                enemyComp.SetTargetTransform(transform);
            }
        }

        DrawDebugBoundary();
    }

    void DrawDebugBoundary()
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            Debug.DrawRay(transform.position, dir * radius, Color.red, 0.5f);
        }
    }
}
