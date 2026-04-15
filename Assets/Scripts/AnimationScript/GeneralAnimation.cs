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

        if (animator != null && controller != null)
        {
            Reflection reflection = controller.GetComponent<Reflection>();
            if (reflection != null)
            {
                reflection.activateReflection += PlayReflectionAnimation;
            }
        }
    }

    private void OnDisable()
    {
        if (animator != null && controller != null)
        {
            Reflection reflection = controller.GetComponent<Reflection>();
            if (reflection != null)
            {
                reflection.activateReflection -= PlayReflectionAnimation;
            }
        }
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

    void PlayReflectionAnimation()
    {
        if(animator != null)
        {
            animator.CrossFade("Reflection", 0.1f);
        }
    }
}
