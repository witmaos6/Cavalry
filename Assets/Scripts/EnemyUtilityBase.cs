using UnityEngine;
using UnityEngine.InputSystem;

public abstract class EnemyUtilityBase : MonoBehaviour
{
    public enum UtilityType { Sensor, Response }

    protected Transform player;
    protected float coolDown = 4.0f;
    protected bool canAcitavte = true;
    public UtilityType utilityType = UtilityType.Sensor;
    

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
