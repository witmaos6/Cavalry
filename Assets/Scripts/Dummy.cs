using System.Collections;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public float lifeTime = 1.0f;
    public float skillCoolTime = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Destroy());

        InvokeRepeating("Skill", 0.0f, skillCoolTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Destroy()
    {
        Destroy(gameObject);

        yield return new WaitForSeconds(lifeTime);
    }

    void Skill()
    {
        // To do: 어그로 변경
    }
}
