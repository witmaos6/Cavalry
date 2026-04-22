using System.Collections;
using UnityEngine;

public class EnemyPredictiveAttack : EnemyAttackBase
{
    public float leadDistance = 2.0f;
    public GameObject bulletPrefab;
    public float preDelay = 1.0f;

    public bool playAnimation = false;

    //private bool activateState = false;

    //void Update() // To do: 추후에 인디케이터로 구현
    //{
    //    if(activateState && player != null)
    //    {
    //        Vector2 playerDirection = player.transform.up;

    //        Vector3 predictedPosition = player.position + (Vector3)(playerDirection * leadDistance);
    //        Vector2 shootDirection = (predictedPosition - transform.position).normalized;

    //        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

    //        Debug.DrawLine(transform.position, predictedPosition, Color.red, 0.1f);
    //    }
    //}
    public override void Attack()
    {
        player = GetPlayerTarget();
        if(player != null)
        {
            if(!playAnimation)
            {
                //activateState = true;
                StartCoroutine(Shot());
            }
            else
            {
                Animator animator = enemy.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.CrossFade("PredictiveAttack", 0.1f);
                }
            }
        }
    }

    IEnumerator Shot()
    {
        yield return new WaitForSeconds(preDelay);
        //activateState = false;

        if(player != null)
        {
            Vector2 playerDirection = player.transform.up;

            Vector3 predictedPosition = player.position + (Vector3)(playerDirection * leadDistance);
            Vector2 shootDirection = (predictedPosition - transform.position).normalized;

            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

            Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle - 90.0f));
        }
    }

    public void ShotObject()
    {
        StartCoroutine(Shot());
    }
}
