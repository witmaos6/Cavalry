using UnityEngine;
using UnityEngine.InputSystem;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public Vector3 offSet = new Vector3(0, 0, -0.1f);
    public float zAngle = 90f;

    private Camera cam;
    void Start()
    {
        if(player == null)
        {
            player = transform.parent.gameObject;
        }
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isGameActive)
            return;

        if (cam == null)
            return;

        transform.position = player.transform.position + offSet;

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        float distanceFormCamera = Mathf.Abs(cam.transform.position.z);
        mouseScreenPos.z = distanceFormCamera;

        Vector3 mousePos = cam.ScreenToWorldPoint(mouseScreenPos);
        Vector2 lookDir = (Vector2)mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle - zAngle);
    }
}
