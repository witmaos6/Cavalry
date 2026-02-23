using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject player;
    public Vector3 offSet = new Vector3(0, 0, -10);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        transform.position = player.transform.position + offSet;
    }
}
