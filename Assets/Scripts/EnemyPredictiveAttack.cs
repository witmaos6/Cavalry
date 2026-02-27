using System.Collections;
using UnityEngine;

public class EnemyPredictiveAttack : EnemyAttackBase
{
    public float leadDistance = 2.0f;
    public GameObject bulletPrefab;
    public float preDelay = 1.0f;
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
            //activateState = true;
            StartCoroutine(Shot());
        }
    }

    IEnumerator Shot()
    {
        yield return new WaitForSeconds(preDelay);
        //activateState = false;

        Vector2 playerDirection = player.transform.up;

        Vector3 predictedPosition = player.position + (Vector3)(playerDirection * leadDistance);
        Vector2 shootDirection = (predictedPosition - transform.position).normalized;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, angle - 90.0f));
    }
}
