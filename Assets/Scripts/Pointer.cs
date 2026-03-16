using UnityEngine;

public class Pointer : MonoBehaviour
{
    private SpriteRenderer sr;

    public Color normalColor = Color.white;
    public Color chargingColor = Color.yellow;
    public Color fullChargeColor = Color.red;
    public Color cooldownColor = Color.black;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isGameActive)
            return;

        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 mouseScreenPos = Input.mousePosition;
        float distanceFromCamera = Mathf.Abs(cam.transform.position.z);
        mouseScreenPos.z = distanceFromCamera;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos;
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
