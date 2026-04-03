using System.Collections;
using UnityEngine;

public class EnemyEvasion : EnemyUtilityBase
{
    public float evasionDistance = 5f;

    private void Start()
    {
        player = GetPlayerTarget();
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.shotArrow += ActivateWithPrediction;
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

    public void ActivateWithPrediction(Vector2 arrowPos, Vector2 arrowDir)
    {
        if (!ActivateCondition())
            return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        Vector2 perpendicularLeft = new Vector2(-arrowDir.y, arrowDir.x);
        Vector2 perpendicularRight = new Vector2(arrowDir.y, arrowDir.x);

        Vector2 toEnemy = (Vector2)transform.position - arrowPos;
        float crossProduct = (arrowDir.x * toEnemy.y) - (arrowDir.y * toEnemy.x);

        Vector2 dodgeDir = (crossProduct > 0) ? perpendicularLeft : perpendicularRight;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dodgeDir * evasionDistance, ForceMode2D.Impulse);

        StartCoroutine(CoolDown());

        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
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

    void Dead()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.shotArrow -= ActivateWithPrediction;
        }
    }
}
