using System.Collections;
using UnityEngine;

public class Reflection : MonoBehaviour
{
    public GameObject hwando;
    public float reflectionCoolDown = 1.0f;
    public float reflectionTime = 0.3f;
    private bool canReflection = true;
    public float offset = 1.0f;
    private GameObject currentHwando;

    // Update is called once per frame
    void Update()
    {
        if(currentHwando != null)
        {
            RotateHwandoTowardMouse();
        }
    }

    public bool ActivateReflection()
    {
        if (!canReflection)
            return false;

        currentHwando = Instantiate(hwando);

        StartCoroutine(Slash());
        StartCoroutine(ReflectionCoolDown());

        return true;
    }

    IEnumerator Slash()
    {
        yield return new WaitForSeconds(reflectionTime);
        Destroy(currentHwando);

        PlayerController playerController = GetComponent<PlayerController>();
        if(playerController != null)
        {
            playerController.ResetActivateHwando();
        }
    }

    IEnumerator ReflectionCoolDown()
    {
        canReflection = false;
        yield return new WaitForSeconds(reflectionCoolDown);
        canReflection = true;
    }

    void RotateHwandoTowardMouse()
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
        currentHwando.transform.position = playerPos + (direction * offset);

        Debug.DrawRay(transform.position, direction * offset, Color.red);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        currentHwando.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
