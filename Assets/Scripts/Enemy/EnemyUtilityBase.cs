using UnityEngine;
using UnityEngine.InputSystem;

public abstract class EnemyUtilityBase : MonoBehaviour
{
    public enum UtilityType { Sensor, Response }

    protected Transform player;
    protected bool canAcitavte = true;
    public UtilityType utilityType = UtilityType.Sensor;
    public float coolDown = 4.0f;


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
