using System.Collections;
using UnityEngine;

public class EnemyPredictiveAttack : EnemyAttackBase
{
    public float leadDistance = 2.0f;
    public GameObject bulletPrefab;
    public float preDelay = 1.0f;

    public bool playAnimation = false;

    private bool activateState = false;
    public GameObject indicator;
    private GameObject indicatorInstance;
    public float indicatorWidth = 0.1f;
    public float indicatorUnitLength = 1.0f;

    void Update()
    {
        if (activateState && player != null)
        {
            Vector2 playerDirection = player.transform.up;
            Vector3 predictedPosition = player.position + (Vector3)(playerDirection * leadDistance);
            Vector2 shootDirection = (predictedPosition - transform.position).normalized;

            float distance = Vector3.Distance(transform.position, predictedPosition);

            if (indicator != null)
            {
                if(indicatorInstance == null)
                {
                    indicatorInstance = Instantiate(indicator);
                }

                indicatorInstance.SetActive(true);
                indicatorInstance.transform.position = transform.position + (Vector3)(shootDirection * (distance * 0.5f));
                indicatorInstance.transform.up = shootDirection;

                float scaleY = indicatorUnitLength > 0f ? distance / indicatorUnitLength : distance;
                indicatorInstance.transform.localScale = new Vector3(indicatorWidth, scaleY, 1f);
            }
        }
        else
        {
            if(indicatorInstance != null && indicatorInstance.activeSelf)
            {
                indicatorInstance.SetActive(false);
            }
        }
    }
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
                    activateState = true;
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
            activateState = false;
        }
    }

    public void ShotObject()
    {
        StartCoroutine(Shot());
    }

    private void OnDisable()
    {
        if(indicatorInstance != null)
        {
            Destroy(indicatorInstance);
            indicatorInstance = null;
        }
    }
}
