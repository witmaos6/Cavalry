using UnityEngine;

public class EnemyAttackBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        Enemy enemy = animator.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.SetAttacking(true);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        Enemy enemy = animator.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.SetAttacking(false);
        }
    }
}
