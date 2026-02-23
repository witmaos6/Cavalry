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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isGameActive)
            return;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = 10f;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = -0.1f;
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
