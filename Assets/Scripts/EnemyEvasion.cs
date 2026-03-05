using System.Collections;
using UnityEngine;

public class EnemyEvasion : EnemyUtilityBase
{
    public float evasionDistance = 10f;

    private void Awake()
    {
        PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.shotArrow += Activate;
        }
    }

    public override bool ActivateCondition()
    {
        player = GetPlayerTarget();
        if (!canAcitavte || player == null)
            return false;

        return true;
    }
    public override void Activate()
    {
        if(!ActivateCondition())
        {
            return;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        // To do: 피하는 방향 감지 추가
        Vector3 rushDir = transform.right;
        Vector2 rushVelocity = rushDir * evasionDistance;
        rb.AddForce(rushVelocity, ForceMode2D.Impulse);

        StartCoroutine(CoolDown());
        
        Enemy enemy = GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.CalledRunningUtility();
        }
    }

    IEnumerator CoolDown()
    {
        canAcitavte = false;
        yield return new WaitForSeconds(coolDown);
        canAcitavte = true;
    }
}
