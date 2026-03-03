using System.Collections;
using UnityEngine;

public class EnemyEvasion : EnemyUtilityBase
{
    public float evasionDistance = 10f;
    public override bool ActivateCondition()
    {
        player = GetPlayerTarget();
        if (!canAcitavte || player == null)
            return false;

        GameObject arrow = GameObject.Find("Arrow");
        if (arrow == null)
            return false;

        return true;
    }
    public override void Activate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        Vector3 rushDir = transform.right;
        Vector2 rushVelocity = rushDir * evasionDistance;
        rb.AddForce(rushVelocity, ForceMode2D.Impulse);

        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        canAcitavte = false;
        yield return new WaitForSeconds(coolDown);
        canAcitavte = true;
    }
}
