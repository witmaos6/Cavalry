using UnityEngine;

public abstract class EnemyUtilityBase : MonoBehaviour
{
    protected Transform player;
    protected float coolDown = 4.0f;
    protected bool canAcitavte = true;

    public abstract bool ActivateCondition();
    public abstract void Activate();

    public Transform GetPlayerTarget()
    {
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            return enemy.GetCurrentTarget();
        }
        return null;
    }
}
