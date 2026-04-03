using System.Collections;
using UnityEngine;

public class EnemyRush : EnemyUtilityBase
{
    public float rushDistance = 10f;

    public override bool ActivateCondition()
    {
        player = GetPlayerTarget();

        if (!canAcitavte || player == null)
            return false;

        // 플레이어와의 거리 측정을 계산한 후 실행할 수 있음 
        return true;
    }
    public override void Activate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        Vector3 rushDir = transform.up;
        Vector2 rushVelocity = rushDir * rushDistance;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(transform.up * rushDistance, ForceMode2D.Impulse);

        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        canAcitavte = false;
        yield return new WaitForSeconds(coolDown);
        canAcitavte = true;
    }
}
