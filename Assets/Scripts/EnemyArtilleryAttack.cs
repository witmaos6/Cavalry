using System.Collections;
using UnityEngine;

public class EnemyArtilleryAttack : EnemyAttackBase
{
    public GameObject ArtilleryBullet;
    
    public float targetRandomRadius = 2.0f;    

    public override void Attack()
    {
        player = GetPlayerTarget();
        if(player != null)
        {
            Vector2 randomOffset = Random.insideUnitCircle * targetRandomRadius;
            Vector3 targetPosition = player.position + (Vector3)randomOffset;

            Instantiate(ArtilleryBullet, targetPosition, Quaternion.identity);
        }
    }
}
