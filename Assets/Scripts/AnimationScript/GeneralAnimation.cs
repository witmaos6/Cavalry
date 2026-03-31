using UnityEngine;

public class GeneralAnimation : MonoBehaviour
{
    private PlayerController controller;
    private Animator animator;

    public string animArrowKey = "IsCharging";

    void Start()
    {
        GameObject parentObject = transform.parent.gameObject;
        controller = parentObject.GetComponent<PlayerController>();

        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (!GameManager.instance.isGameActive)
            return;

        if(controller.IsCharging())
        {
            animator.SetBool(animArrowKey, true);
        }
        else
        {
            animator.SetBool(animArrowKey, false);
        }
    }
}
