using System.Collections;
using UnityEngine;

public class Guard : MonoBehaviour
{
    // Use Player and Enemy
    public GameObject shield;
    public float guardTime = 0.5f;
    public float offset = 1.0f;
    public float guardCoolDown = 1.0f;
    private bool canGuard = true;

    private GameObject currentShield;

    void Start()
    {
    }

    void Update()
    {
        if (currentShield != null)
        {
            RotateShieldTowardMouse();
        }
    }

    public bool ActivateGuard()
    {
        if (!canGuard)
            return false;

        currentShield = Instantiate(shield);

        StartCoroutine(ShieldLifetime());
        StartCoroutine(GuardCooldown());

        return true;
    }

    IEnumerator GuardCooldown()
    {
        canGuard = false;
        yield return new WaitForSeconds(guardCoolDown);
        canGuard = true;
    }

    IEnumerator ShieldLifetime()
    {
        yield return new WaitForSeconds(guardTime);
        Destroy(currentShield);

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.ResetActivateHwando();
        }
    }

    void RotateShieldTowardMouse()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 mouseScreenPos = Input.mousePosition;
        float distanceFromCamera = Mathf.Abs(cam.transform.position.z);
        mouseScreenPos.z = distanceFromCamera;

        Vector3 mousePos = cam.ScreenToWorldPoint(mouseScreenPos);
        mousePos.z = 0.0f;

        Vector3 playerPos = transform.position;
        Vector3 direction = (mousePos - playerPos).normalized;
        currentShield.transform.position = playerPos + (direction * offset);

        Debug.DrawRay(transform.position, direction * offset, Color.red);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        currentShield.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
