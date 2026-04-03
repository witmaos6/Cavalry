using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject player;
    public Vector3 offSet = new Vector3(0, 0, -10);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        {
            player = GameObject.Find("Player");
        }
    }

    private void LateUpdate()
    {
        transform.position = player.transform.position + offSet;
    }
}
