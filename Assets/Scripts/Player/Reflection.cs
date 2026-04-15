using System.Collections;
using UnityEngine;

public class Reflection : MonoBehaviour
{
    public delegate void Activate();
    public Activate activateReflection;

    public GameObject hwando;
    public float reflectionCoolDown = 1.0f;
    public float reflectionTime = 0.3f;
    private bool canReflection = true;
    public float offset = 1.0f;
    private GameObject currentHwando;

    public bool ActivateReflection()
    {
        if (!canReflection)
            return false;

        currentHwando = Instantiate(hwando, transform.position, transform.rotation);
        activateReflection?.Invoke();

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
}
