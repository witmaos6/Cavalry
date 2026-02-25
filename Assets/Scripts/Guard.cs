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
        if (!GameManager.Instance.isGameActive)
        {
            return;
        }
        if (canGuard)
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentShield = Instantiate(shield);

                StartCoroutine(ShieldLifetime());
                StartCoroutine(GuardCooldown());
            }
        }
        if(currentShield != null)
        {
            RotateShieldTowardMouse();
        }
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
