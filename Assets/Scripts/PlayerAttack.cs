using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Referência")]
    public GameObject hitbox;
    public PlayerController_BeatEmUp controller;

    [Header("Ataque")]
    public KeyCode attackKey = KeyCode.J;
    public float activeTime = 0.12f;
    public float cooldown = 0.25f;

    [Header("Input Buffer")]
    public float inputBufferTime = 0.18f;

    [Header("Combo")]
    public float comboResetTime = 0.8f;
    public int comboStep = 0;

    [Header("Hitbox Offset (X)")]
    public float hitboxOffsetX = 0.8f;

    bool canAttack = true;
    bool attackBuffered = false;
    bool isAttacking = false;

    float comboTimer;
    float bufferTimer;

    HitboxDamage hitboxDamage;

    void Awake()
    {
        if (controller == null)
            controller = GetComponent<PlayerController_BeatEmUp>();

        if (hitbox != null)
        {
            hitbox.SetActive(false);
            hitboxDamage = hitbox.GetComponent<HitboxDamage>();
        }
    }

    void Update()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;

            if (comboTimer <= 0f)
            {
                comboStep = 0;
                Debug.Log("Combo resetou.");
            }
        }

        if (bufferTimer > 0f)
        {
            bufferTimer -= Time.deltaTime;

            if (bufferTimer <= 0f)
                attackBuffered = false;
        }

        if (Input.GetKeyDown(attackKey))
        {
            if (canAttack && !isAttacking)
            {
                StartCoroutine(DoAttack());
            }
            else
            {
                attackBuffered = true;
                bufferTimer = inputBufferTime;
            }
        }
    }

    IEnumerator DoAttack()
    {
        isAttacking = true;
        canAttack = false;
        attackBuffered = false;
        bufferTimer = 0f;

        PlayerParry parry = GetComponent<PlayerParry>();
        bool counterAttack = false;

        if (parry != null && parry.counterWindow)
        {
            counterAttack = true;
            parry.counterWindow = false;
        }

        comboStep++;
        if (comboStep > 3)
            comboStep = 1;

        comboTimer = comboResetTime;

        Debug.Log("Combo Step = " + comboStep);

        if (hitbox != null)
        {
            int dir = (controller != null) ? controller.FacingDir : 1;

            Vector3 p = hitbox.transform.localPosition;
            p.x = Mathf.Abs(hitboxOffsetX) * dir;
            hitbox.transform.localPosition = p;

            if (hitboxDamage != null)
            {
                if (counterAttack)
                {
                    Debug.Log("COUNTER ATTACK!");
                    hitboxDamage.damage = 2;
                    hitboxDamage.knockbackForce = 16f;
                    hitboxDamage.finisherHit = true;
                    hitboxDamage.knockdownHit = true;

                    // 🔥 IMPACTO FORTE
                    CameraShake.I?.Shake(0.1f, 0.15f);
                    StartCoroutine(HitStop(0.05f));
                }
                else if (comboStep == 1)
                {
                    hitboxDamage.damage = 1;
                    hitboxDamage.knockbackForce = 8f;
                    hitboxDamage.finisherHit = false;
                    hitboxDamage.knockdownHit = false;

                    // 🔥 IMPACTO LEVE
                    CameraShake.I?.Shake(0.05f, 0.06f);
                    StartCoroutine(HitStop(0.02f));
                }
                else if (comboStep == 2)
                {
                    hitboxDamage.damage = 1;
                    hitboxDamage.knockbackForce = 9f;
                    hitboxDamage.finisherHit = false;
                    hitboxDamage.knockdownHit = false;

                    CameraShake.I?.Shake(0.06f, 0.07f);
                    StartCoroutine(HitStop(0.025f));
                }
                else if (comboStep == 3)
                {
                    hitboxDamage.damage = 2;
                    hitboxDamage.knockbackForce = 11f;
                    hitboxDamage.finisherHit = true;
                    hitboxDamage.knockdownHit = true;

                    // 🔥 IMPACTO FORTE
                    CameraShake.I?.Shake(0.1f, 0.12f);
                    StartCoroutine(HitStop(0.04f));
                }
            }

            hitbox.SetActive(true);
        }

        yield return new WaitForSeconds(activeTime);

        if (hitbox != null)
            hitbox.SetActive(false);

        isAttacking = false;
        StartCoroutine(AttackRecovery());
    }

    IEnumerator AttackRecovery()
    {
        yield return new WaitForSeconds(cooldown * 0.6f);

        canAttack = true;

        if (attackBuffered)
        {
            StartCoroutine(DoAttack());
        }
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}