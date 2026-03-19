using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.UI;
using static GameData;


public class PlayerController : MonoBehaviour
{
    public delegate void ShotArrow(Vector2 position, Vector2 direction);
    public ShotArrow shotArrow;
    public enum HwandoType {Guard, Reflection }

    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    private Vector2 lastMoveDir;

    [Header("Attack Settings")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public Pointer pointer;
    public float fireRate = 1.0f;
    private bool canAttack = true;
    private float chargeTimer = 0;
    private float maxChargeTime = 1.5f;
    public float attackPower = 2.0f;

    [Header("Dash Settings")]
    public float dashAmount = 10.0f;
    public float dashCooldown = 1.0f;
    private bool canDash = true;

    [Header("Dummy Settings")]
    public float dummyCooldown = 1.0f;
    public GameObject dummy;
    private bool canDummy = true;

    [Header("Hwando Settings")]
    public HwandoType hwandoType = HwandoType.Guard; // To do: 삭제 예정
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        InputManager.instance.controls.Player.ArrowCharge.started += OnArrowStarted;
        InputManager.instance.controls.Player.ArrowCharge.canceled += OnArrowCanceled;
        InputManager.instance.controls.Player.Hwando.started += OnHwandoStarted;
        InputManager.instance.controls.Player.Dummy.started += OnDummyStarted;
        InputManager.instance.controls.Player.Dash.started += OnDashStarted;
        
    }

    private void OnDisable()
    {
        InputManager.instance.controls.Player.ArrowCharge.started -= OnArrowStarted;
        InputManager.instance.controls.Player.ArrowCharge.canceled -= OnArrowCanceled;
        InputManager.instance.controls.Player.Hwando.started -= OnHwandoStarted;
        InputManager.instance.controls.Player.Dummy.started -= OnDummyStarted;
        InputManager.instance.controls.Player.Dash.started -= OnDashStarted;
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
        if (hpSlider != null)
        {
            hpSlider.gameObject.SetActive(false);
            hpSlider.maxValue = hp;
            hpSlider.value = hp;
        }
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
            Movement();

            ChargeArrow();   
        }
    }

    void Movement()
    {
        Vector2 inputVec = controls.Player.Move.ReadValue<Vector2>();

        if(inputVec.sqrMagnitude > 0)
        {
            Vector2 targetDir = inputVec.normalized;
            lastMoveDir = Vector2.Lerp(lastMoveDir, targetDir, 0.1f);

            float angle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90.0f);
        }
        Vector3 nextPosition = transform.position + (Vector3)(lastMoveDir * moveSpeed * Time.deltaTime);

        transform.position = nextPosition;
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

        CreateArrow(firePoint.rotation, isFull, power);

        shotArrow?.Invoke(firePoint.position, firePoint.up);

        if (skillUnlockStatus[SkillID.MultipleShot])
        {
            Quaternion leftRot = firePoint.rotation * Quaternion.Euler(0, 0, 30f);
            CreateArrow(leftRot, isFull, power);

            Quaternion rightRot = firePoint.rotation * Quaternion.Euler(0, 0, -30f);
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

        if (chargeTimer > 0.0f) // 활시위를 당기고 있을 때 환도 사용 불가
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
        StartCoroutine(AttackPowerBuff(buffDurationTime, amountAttackPower));
    }

    IEnumerator AttackPowerBuff(float buffDurationTime, float amountAttackPower)
    {
        attackPower += amountAttackPower;
        yield return new WaitForSeconds(buffDurationTime);
        attackPower -= amountAttackPower;
    }
}
