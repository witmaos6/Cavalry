using UnityEngine;

public class HorseAnimation : MonoBehaviour
{
    private PlayerController controller;
    private Animator animator;

    private float maxSpeed = 0f;

    public string animSpeedKey = "AnimSpeed";

    void Start()
    {
        GameObject parentObject = transform.parent.gameObject;
        controller = parentObject.GetComponent<PlayerController>();
        maxSpeed = controller.moveSpeed;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!GameManager.instance.isGameActive)
            return;

        float currentSpeed = controller.GetCurrentSpeed();
        animator.SetFloat(animSpeedKey, currentSpeed / maxSpeed);
        // To do: 회전 스탑 등 다른 애니메이션 적용이 생길 경우 수정
    }
}
