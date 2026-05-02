using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static GameData;


public class PlayerController : MonoBehaviour
{
    public delegate void ShotArrow(Vector2 position, Vector2 direction);
    public ShotArrow shotArrow;

    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float acceleration = 10f;
    private Vector2 lastMoveDir = Vector2.up;
    private float currentSpeed = 0f;

    [Header("Attack Settings")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public Pointer pointer;
    public float fireRate = 1.0f;
    private bool canAttack = true;
    private float chargeTimer = 0;
    private float maxChargeTime = 1.5f;
    public float attackPower = 2.0f;
    public GameObject attackBuffPrefab;
    private GameObject attackBuffInstance;

    [Header("Dash Settings")]
    public float dashAmount = 10.0f;
    public float dashCooldown = 1.0f;
    private bool canDash = true;

    [Header("Dummy Settings")]
    public float dummyCooldown = 1.0f;
    public GameObject dummy;
    private bool canDummy = true;

    [Header("Hwando Settings")]
    private Guard guard;
    private Reflection reflection;
    private bool activateHwando = false;

    [Header("Stat Settings")]
    public float hp = 10;
    private bool isDead = false;

    [Header("UI Settings")]
    public Slider hpSlider;

    private bool isCharging = false;
    private Rigidbody2D rb;
    private Dictionary<SkillID, bool> skillUnlockStatus = new Dictionary<SkillID, bool>();

    PlayerControls controls;

    Coroutine attackCoolDownCoroutine;
    Coroutine attackPowerBuffCoroutine;
    Transform startTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        guard = GetComponent<Guard>();
        reflection = GetComponent<Reflection>();

        InitInputSet();

        InitSkillSet();
    }

    void InitInputSet()
    {
        controls = InputManager.instance.controls;
        controls.Enable();
    }

    private void OnEnable()
    {
        controls.Player.ArrowCharge.started += OnArrowStarted;
        controls.Player.ArrowCharge.canceled += OnArrowCanceled;
        controls.Player.Hwando.started += OnHwandoStarted;
        controls.Player.Dummy.started += OnDummyStarted;
        controls.Player.Dash.started += OnDashStarted;
        
    }

    private void OnDisable()
    {
        controls.Player.ArrowCharge.started -= OnArrowStarted;
        controls.Player.ArrowCharge.canceled -= OnArrowCanceled;
        controls.Player.Hwando.started -= OnHwandoStarted;
        controls.Player.Dummy.started -= OnDummyStarted;
        controls.Player.Dash.started -= OnDashStarted;
    }

    void InitSkillSet()
    {
        PlayerSkillManager skillManager = GetComponent<PlayerSkillManager>();
        if (skillManager == null)
            return;
        
        GameData gameData = skillManager.GetGameData();
        if (gameData == null)
            return;

        foreach (SkillID skillId in Enum.GetValues(typeof(SkillID)))
        {
            skillUnlockStatus[skillId] = gameData.skillSet.Contains(skillId);
        }
    }

    void OnArrowStarted(InputAction.CallbackContext context)
    {
        if (!GameManager.instance.isGameActive)
            return;

        if (canAttack && !activateHwando)
        {
            isCharging = true;
        }
    }

    void OnArrowCanceled(InputAction.CallbackContext context)
    {
        if (!GameManager.instance.isGameActive)
            return;

        if (isCharging)
        {
            AttackArrow();
        }
    }

    void OnHwandoStarted(InputAction.CallbackContext context)
    {
        if (GameManager.instance.isGameActive)
        {
            ActivateHwando();
        }
    }

    void OnDummyStarted(InputAction.CallbackContext context)
    {
        if (GameManager.instance.isGameActive)
        {
            SpawnDummy();
        }
    }

    void OnDashStarted(InputAction.CallbackContext context)
    {
        if (GameManager.instance.isGameActive)
        {
            Dash();
        }
    }

    void Start()
    {
        startTransform = transform;

        StartSet();
    }

    public void StartSet()
    {
        if (hpSlider != null)
        {
            hpSlider.gameObject.SetActive(false);
            hpSlider.maxValue = hp;
            hpSlider.value = hp;
        }

        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;
        transform.localScale = startTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isGameActive)
        {
            return;
        }
        if (!isDead)
        {
            CalculateMovementLogic();

            ChargeArrow();   
        }
    }

    void CalculateMovementLogic()
    {
        Vector2 inputVec = controls.Player.Move.ReadValue<Vector2>();

        if(inputVec.sqrMagnitude > 0)
        {
            Vector2 targetDir = inputVec.normalized;
            float angleDiff = Vector2.Angle(lastMoveDir.normalized, targetDir);

            float penalty = TurningPanelty(angleDiff);
            float penelizedSpeed = moveSpeed * penalty;
            if(currentSpeed > penelizedSpeed)
            {
                currentSpeed = penelizedSpeed;
            }

            lastMoveDir = Vector2.Lerp(lastMoveDir, targetDir, 0.15f);
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, acceleration * Time.deltaTime);

        float rotationAngle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotationAngle - 90f);
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isGameActive || isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = lastMoveDir.normalized * currentSpeed;
    }

    float TurningPanelty(float angleDiff)
    {
        if (angleDiff <= 10f)
        {
            return 1f;
        }
        else if (angleDiff <= 90f)
        {
            float t = (angleDiff - 10f) / (90f - 10f);
            return Mathf.Lerp(1.0f, 0.5f, t);
        }
        float temp = (angleDiff - 90f) / (180f - 90f);
        return Mathf.Lerp(0.5f, 0.1f, temp);
    }

    void ChargeArrow()
    {
        if (isCharging && chargeTimer <= maxChargeTime)
        {
            chargeTimer += Time.deltaTime;
            float ratio = Mathf.Clamp01(chargeTimer / maxChargeTime);
            pointer.SetColor(ratio, chargeTimer >= maxChargeTime);
        }
    }

    void AttackArrow()
    {
        bool isFull = chargeTimer >= maxChargeTime;
        float power = Mathf.Clamp(chargeTimer, 0.5f, 1.5f);
        attackCoolDownCoroutine = StartCoroutine(AttackCooldownTimer(isFull));

        isCharging = false;
        chargeTimer = 0f;
        pointer.SetCooldown(true);

        Vector3 dir = (pointer.transform.position - firePoint.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle - 90f);

        CreateArrow(rot, isFull, power);

        shotArrow?.Invoke(firePoint.position, firePoint.up);

        if (skillUnlockStatus[SkillID.MultipleShot])
        {
            Quaternion leftRot = rot * Quaternion.Euler(0, 0, 30f);
            CreateArrow(leftRot, isFull, power);

            Quaternion rightRot = rot * Quaternion.Euler(0, 0, -30f);
            CreateArrow(rightRot, isFull, power);
        }
    }

    void CreateArrow(Quaternion rotation, bool isFull, float power)
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, rotation);
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.Setup(isFull, power, gameObject);
        }
    }

    IEnumerator AttackCooldownTimer(bool isFull)
    {
        canAttack = false;

        float coolDown = isFull ? fireRate : fireRate / 2;
        yield return new WaitForSeconds(coolDown);

        canAttack = true;
        pointer.SetCooldown(false);
    }

    public void ResetAttackCoolDown()
    {
        if(attackCoolDownCoroutine != null)
        {
            StopCoroutine(attackCoolDownCoroutine);
            attackCoolDownCoroutine = null;

            canAttack = true;
            pointer.SetCooldown(false);
        }
    }

    void Dash()
    {
        if (!skillUnlockStatus[SkillID.Dash])
            return;

        if (!canDash)
            return;

        rb.AddForce(transform.up * dashAmount, ForceMode2D.Impulse);

        StartCoroutine(DashCooldownTimer());
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

    void SpawnDummy()
    {
        if (!skillUnlockStatus[SkillID.Dummy])
            return;

        if (!canDummy)
            return;

        GameObject spawnedDummy = Instantiate(dummy, pointer.transform.position, pointer.transform.rotation);
        if (spawnedDummy != null)
        {
            Dummy dummyComp = spawnedDummy.GetComponent<Dummy>();
            if (dummyComp != null)
            {
                dummyComp.SetOwner(gameObject);
            }
        }

        StartCoroutine(DummyCoolDown());
    }

    IEnumerator DummyCoolDown()
    {
        canDummy = false;
        yield return new WaitForSeconds(dummyCooldown);
        canDummy = true;
    }

    void ActivateHwando()
    {
        if (!skillUnlockStatus[SkillID.Guard] && !skillUnlockStatus[SkillID.Reflection])
            return;

        if (isCharging)
            return;

        if (skillUnlockStatus[SkillID.Guard])
        {
            activateHwando = guard.ActivateGuard();
        }
        else if (skillUnlockStatus[SkillID.Reflection])
        {
            activateHwando = reflection.ActivateReflection();
        }
    }

    public void ResetActivateHwando()
    {
        activateHwando = false;
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

            GameManager.instance.GameOver();
        }
    }

    public bool IsUnlockSkill(SkillID skillID)
    {
        return skillUnlockStatus[skillID];
    }

    public void SkillUnlock(SkillID skillID)
    {
        skillUnlockStatus[skillID] = true;
    }

    public void SkillLock(SkillID skillID)
    {
        skillUnlockStatus[skillID] = false;
    }

    public void IncreaseAttackPowerBuff(float buffDurationTime, float amountAttackPower)
    {
        if (attackPowerBuffCoroutine != null)
        {
            StopCoroutine(attackPowerBuffCoroutine);

            ResetAttackPowerBuff(amountAttackPower); 
        }
        attackPowerBuffCoroutine = StartCoroutine(AttackPowerBuff(buffDurationTime, amountAttackPower));
    }

    IEnumerator AttackPowerBuff(float buffDurationTime, float amountAttackPower)
    {
        SetAttackPower(attackPower + amountAttackPower);
        
        if(attackBuffPrefab != null && attackBuffInstance == null)
        {
            attackBuffInstance = Instantiate(attackBuffPrefab, transform.position, transform.rotation);
            if (attackBuffInstance != null)
            {
                attackBuffInstance.transform.SetParent(transform, true);
                attackBuffInstance.transform.position = new Vector3(attackBuffInstance.transform.position.x, attackBuffInstance.transform.position.y, attackBuffInstance.transform.position.z - 0.2f);
            }
        }

        yield return new WaitForSeconds(buffDurationTime);
        ResetAttackPowerBuff(amountAttackPower);
    }

    void ResetAttackPowerBuff(float amountAttackPower)
    {
        SetAttackPower(attackPower - amountAttackPower);

        if (attackBuffInstance != null)
        {
            Destroy(attackBuffInstance);
            attackBuffInstance = null;
        }
        attackPowerBuffCoroutine = null;
    }

    void SetAttackPower(float inAttackPower)
    {
        attackPower = inAttackPower;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public bool IsCharging()
    {
        return isCharging;
    }
}
