using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject enemyHitbox;

    [Header("Ataque")]
    public float attackRange = 1.6f;
    public float activeTime = 0.12f;
    public float cooldown = 1.0f;

    [Header("Attack Tuning")]
    public float attackStartBuffer = 0.35f;

    [Header("Attack Delay")]
    public float minAttackDelay = 0.25f;
    public float maxAttackDelay = 0.6f;

    [Header("Telegraph")]
    public float telegraphTime = 0.35f;
    public Color telegraphColor = Color.red;

    [Header("Miss Reaction")]
    public float missBackstepDistance = 0.8f;
    public float missBackstepTime = 0.15f;
    public float missRecoveryTime = 0.35f;

    [Header("Parry Reaction")]
    public float parryStunTime = 0.8f;

    [Header("Parry Knockback")]
    public float parryKnockbackForce = 6f;
    public float parryKnockbackTime = 0.1f;

    Transform player;
    Rigidbody2D rb;
    SpriteRenderer sr;
    EnemyHealth eh;
    EnemyChase chase;
    SFXManager sfxManager;

    bool canAttack = true;
    bool hitConnected = false;
    bool preparingAttack = false;
    bool isParryStunned = false;
    bool isTelegraphing = false;
    bool alreadyParried = false;

    Color baseColor = Color.white;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        eh = GetComponent<EnemyHealth>();
        chase = GetComponent<EnemyChase>();
        sfxManager = FindFirstObjectByType<SFXManager>();

        if (sr != null)
            baseColor = sr.color;
    }

    void Start()
    {
        if (enemyHitbox != null)
            enemyHitbox.SetActive(false);

        GameObject p = GameObject.Find("Player");
        if (p != null)
            player = p.transform;
    }

    void OnDisable()
    {
        CleanupAttackState();
    }

    void OnDestroy()
    {
        CleanupAttackState();
    }

    void CleanupAttackState()
    {
        StopAllCoroutines();

        if (enemyHitbox != null)
            enemyHitbox.SetActive(false);

        preparingAttack = false;
        isParryStunned = false;
        isTelegraphing = false;
        hitConnected = false;
        canAttack = true;
        alreadyParried = false;

        ResetVisual();

        if (EnemyAttackManager.I != null)
            EnemyAttackManager.I.StopAttack(gameObject);
    }

    void ResetVisual()
    {
        if (sr != null)
            sr.color = baseColor;
    }

    void Update()
    {
        if (eh != null && eh.isKnockedDown) return;
        if (isParryStunned) return;
        if (player == null) return;
        if (!canAttack) return;
        if (preparingAttack) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange + attackStartBuffer)
        {
            if (EnemyAttackManager.I != null &&
                EnemyAttackManager.I.CanAttack(gameObject))
            {
                StartCoroutine(PrepareAttack());
            }
        }
    }

    IEnumerator PrepareAttack()
    {
        preparingAttack = true;

        float delay = Random.Range(minAttackDelay, maxAttackDelay);
        yield return new WaitForSeconds(delay);

        if (!canAttack || player == null || isParryStunned || (eh != null && eh.isKnockedDown))
        {
            preparingAttack = false;

            if (EnemyAttackManager.I != null)
                EnemyAttackManager.I.StopAttack(gameObject);

            yield break;
        }

        isTelegraphing = true;

        if (sr != null)
            sr.color = telegraphColor;

        yield return new WaitForSeconds(telegraphTime);

        ResetVisual();
        isTelegraphing = false;
        preparingAttack = false;

        if (!canAttack || player == null || isParryStunned || (eh != null && eh.isKnockedDown))
        {
            if (EnemyAttackManager.I != null)
                EnemyAttackManager.I.StopAttack(gameObject);

            yield break;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange + attackStartBuffer)
        {
            StartCoroutine(DoAttack());
        }
        else
        {
            if (EnemyAttackManager.I != null)
                EnemyAttackManager.I.StopAttack(gameObject);
        }
    }

    IEnumerator DoAttack()
    {
        canAttack = false;
        hitConnected = false;

        if (enemyHitbox != null)
            enemyHitbox.SetActive(true);

        yield return new WaitForSeconds(activeTime);

        if (enemyHitbox != null)
            enemyHitbox.SetActive(false);

        if (!hitConnected)
            yield return StartCoroutine(DoMissReaction());

        yield return new WaitForSeconds(cooldown);

        if (EnemyAttackManager.I != null)
            EnemyAttackManager.I.StopAttack(gameObject);

        canAttack = true;
    }

    IEnumerator DoMissReaction()
    {
        if (rb == null || player == null) yield break;

        if (chase != null)
            chase.movementLocked = true;

        Vector2 away = ((Vector2)transform.position - (Vector2)player.position).normalized;
        if (away == Vector2.zero)
            away = Vector2.right;

        float timer = 0f;

        while (timer < missBackstepTime)
        {
            rb.linearVelocity = away * (missBackstepDistance / missBackstepTime);
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(missRecoveryTime);

        if (chase != null)
            chase.movementLocked = false;
    }

    IEnumerator HitInterruptRecover()
    {
        canAttack = false;
        yield return new WaitForSeconds(0.2f);
        canAttack = true;
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    IEnumerator ParryKnockback()
    {
        if (rb == null || player == null)
            yield break;

        Vector2 direction = ((Vector2)transform.position - (Vector2)player.position).normalized;

        if (direction == Vector2.zero)
            direction = Vector2.right;

        float timer = 0f;

        while (timer < parryKnockbackTime)
        {
            rb.linearVelocity = direction * parryKnockbackForce;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
    }

    IEnumerator ParryStunRoutine()
    {
        isParryStunned = true;
        canAttack = false;
        preparingAttack = false;
        isTelegraphing = false;

        Color old = baseColor;

        if (sr != null)
        {
            old = sr.color;
            sr.color = Color.yellow;
        }

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (EnemyAttackManager.I != null)
            EnemyAttackManager.I.StopAttack(gameObject);

        Debug.Log(gameObject.name + " foi defendido no parry!");

        yield return new WaitForSeconds(parryStunTime);

        if (sr != null)
            sr.color = old == telegraphColor ? baseColor : old;

        ResetVisual();

        isParryStunned = false;
        canAttack = true;
        alreadyParried = false;
    }

    public void ResetForSpawn()
    {
        StopAllCoroutines();

        if (enemyHitbox != null)
            enemyHitbox.SetActive(false);

        canAttack = true;
        hitConnected = false;
        preparingAttack = false;
        isParryStunned = false;
        isTelegraphing = false;
        alreadyParried = false;

        ResetVisual();

        if (EnemyAttackManager.I != null)
            EnemyAttackManager.I.StopAttack(gameObject);
    }

    public void NotifyHitConnected()
    {
        hitConnected = true;
    }

    public void InterruptAttack()
    {
        StopAllCoroutines();

        if (enemyHitbox != null)
            enemyHitbox.SetActive(false);

        preparingAttack = false;
        isTelegraphing = false;
        hitConnected = false;

        ResetVisual();

        if (EnemyAttackManager.I != null)
            EnemyAttackManager.I.StopAttack(gameObject);

        StartCoroutine(HitInterruptRecover());
    }

    public void OnParried()
    {
        if (alreadyParried) return;
        alreadyParried = true;

        StopAllCoroutines();

        if (enemyHitbox != null)
            enemyHitbox.SetActive(false);

        if (sr != null)
            sr.color = Color.white;

        if (sfxManager != null)
            sfxManager.PlayParry();

        StartCoroutine(HitStop(0.05f));
        StartCoroutine(ParryKnockback());
        StartCoroutine(ParryStunRoutine());
    }
}