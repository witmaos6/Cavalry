using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(100)]
public class Pointer : MonoBehaviour
{
    private SpriteRenderer sr;

    public Color normalColor = Color.white;
    public Color chargingColor = Color.yellow;
    public Color fullChargeColor = Color.red;
    public Color cooldownColor = Color.black;

    private Camera cam;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnRenderObject()
    {
        
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isGameActive)
            return;

        if (cam == null)
        {
            cam = Camera.main;
        }

        if (Mouse.current == null)
            return;

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();

        float distanceFromCamera = Mathf.Abs(cam.transform.position.z);
        mouseScreenPos.z = distanceFromCamera;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;
        transform.localPosition = transform.parent.InverseTransformPoint(mouseWorldPos);
    }

    public void SetColor(float chargeRatio, bool isFull)
    {
        if(isFull)
        {
            sr.color = fullChargeColor;
        }
        else
        {
            sr.color = Color.Lerp(normalColor, chargingColor, chargeRatio);
        }
    }

    public void SetCooldown(bool isCooldown)
    {
        if(isCooldown)
        {
            sr.color = cooldownColor;
        }
        else
        {
            sr.color = normalColor;
        }
    }
}
