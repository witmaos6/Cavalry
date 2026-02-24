using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    private Vector2 lastMoveDir;

    private Rigidbody2D rb;

    [Header("Attack Settings")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public Pointer pointer;
    public float fireRate = 1.0f;
    private bool canAttack = true;
    private float chargeTimer = 0;
    private float maxChargeTime = 1.5f;

    [Header("Dash Settings")]
    public float dashAmount = 10.0f;
    public float dashCooldown = 1.0f;
    private bool canDash = true;

    [Header("Dummy Settings")]
    public float dummyCooldown = 1.0f;
    public GameObject dummy;
    private bool canDummy = true;

    [Header("Stat Settings")]
    public float hp = 10;
    private bool isDead = false;

    [Header("UI Settings")]
    public Slider hpSlider;

    private bool isCharging = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(hpSlider != null)
        {
            hpSlider.gameObject.SetActive(false);
            hpSlider.maxValue = hp;
            hpSlider.value = hp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isGameActive)
        {
            return;
        }
        if (!isDead)
        {
            Movement();

            Attack();

            Dash();

            Dummy();
        }
    }

    void Movement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if(h != 0 || v != 0)
        {
            Vector2 targetDir = new Vector2(h, v).normalized;
            lastMoveDir = Vector2.Lerp(lastMoveDir, targetDir, 0.5f);

            float angle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90.0f);
        }
        Vector3 nextPosition = transform.position + (Vector3)(lastMoveDir * moveSpeed * Time.deltaTime);

        transform.position = nextPosition;
    }

    void Attack()
    {
        if (!canAttack)
            return;

        if(Input.GetMouseButtonDown(1))
        {
            isCharging = true;
        }

        if(isCharging)
        {
            chargeTimer += Time.deltaTime;
            float ratio = Mathf.Clamp01(chargeTimer / maxChargeTime);
            pointer.SetColor(ratio, chargeTimer >= maxChargeTime);

            if(Input.GetMouseButtonUp(1))
            {
                Fire(chargeTimer >= maxChargeTime);
                isCharging = false;
                chargeTimer = 0f;

                pointer.SetCooldown(true);

                StartCoroutine(AttackCooldownTimer());
            }
        }
    }

    void Fire(bool isFull)
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            float power = Mathf.Clamp(chargeTimer, 0.5f, 1.5f);
            arrowScript.Setup(isFull, power);
        }
    }
    IEnumerator AttackCooldownTimer()
    {
        canAttack = false;

        yield return new WaitForSeconds(fireRate);

        canAttack = true;
        pointer.SetCooldown(false);
    }

    void Dash()
    {
        if (!canDash)
            return;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * dashAmount, ForceMode2D.Impulse);

            StartCoroutine(DashCooldownTimer());
        }
    }

    IEnumerator DashCooldownTimer()
    {
        float originDamping = rb.linearDamping;
        rb.linearDamping = 10.0f;

        canDash = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;

        rb.linearDamping = originDamping;
    }

    void Dummy()
    {
        if (!canDummy)
            return;

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            Instantiate(dummy, pointer.transform.position, pointer.transform.rotation);

            StartCoroutine(DummyCoolDown());
        }
    }

    IEnumerator DummyCoolDown()
    {
        canDummy = false;
        yield return new WaitForSeconds(dummyCooldown);
        canDummy = true;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if(hpSlider != null)
        {
            hpSlider.value = hp;
        }
        if (hp <= 0 && !isDead)
        {
            isDead = true;

            GameManager.Instance.GameOver();
        }
    }
}
