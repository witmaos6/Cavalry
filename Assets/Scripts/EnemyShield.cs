using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyShield : EnemyUtilityBase
{
    public GameObject shield;
    public float interceptSpeed = 15f;
    public float shieldDuration = 0.5f;
    public float protectRange = 10f;
    Rigidbody2D rb;

    private void Start()
    {
        player = GetPlayerTarget();
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.shotArrow += ActivateIntercept;
            }
        }

        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.dead += Dead;
        }
    }
    public override bool ActivateCondition()
    {
        player = GetPlayerTarget();
        if (!canAcitavte || player == null)
            return false;

        return true;
    }

    public override void Activate() { }

    public void ActivateIntercept(Vector2 arrowPos, Vector2 arrowDir)
    {
        if (!ActivateCondition())
            return;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        Transform target = FindEndanger(arrowPos, arrowDir);
        if (target != null)
        {
            Vector2 toPlayerDir = (arrowPos - (Vector2)target.position).normalized;
            Vector2 interceptPoint = (Vector2)target.position + toPlayerDir * 1.5f;

            StartCoroutine(GuardianDoutine(interceptPoint, arrowPos));

            Enemy enemy = GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.CalledRunningUtility();
            }
        }
    }

    private Transform FindEndanger(Vector2 arrowPos, Vector2 arrowDir)
    {
        Collider2D[] Find = Physics2D.OverlapCircleAll(transform.position, protectRange, LayerMask.GetMask("Enemy"));
        float minDot = float.MaxValue;
        Transform closestEndangerd = null;

        foreach (var enemy in Find)
        {
            if (enemy.gameObject == this.gameObject)
                continue;

            Vector2 enemyPos = enemy.transform.position;
            Vector2 toEnemy = enemyPos - arrowPos;

            float dot = Vector2.Dot(toEnemy, arrowDir);
            if(dot > 0)
            {
                Vector2 projection = arrowPos + arrowDir * dot;
                float distToPath = Vector2.Distance(enemyPos, projection);

                if(dot < minDot)
                {
                    minDot = dot;
                    closestEndangerd = enemy.transform;
                }
            }
        }
        return closestEndangerd;
    }

    IEnumerator GuardianDoutine(Vector2 movePoint, Vector2 arrowPos)
    {
        rb.linearVelocity = Vector2.zero;

        Vector2 dir = (movePoint - (Vector2)transform.position).normalized;
        rb.AddForce(dir * interceptSpeed, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.15f);
        rb.linearVelocity *= 0.1f;

        if(shield != null)
        {
            shield.SetActive(true); // To do: 방어효과 활성화/비활성화 하는 방식으로 변경 할 수 있음

            Vector2 toPlayer = (arrowPos - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
            shield.transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return new WaitForSeconds(shieldDuration);
            shield.SetActive(false);
        }

        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        canAcitavte = false;
        yield return new WaitForSeconds(coolDown);
        canAcitavte = true;
    }

    void Dead()
    {
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.shotArrow -= ActivateIntercept;
            }
        }
    }
}
