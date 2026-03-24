using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHealth = 7;

    private int currentHealth;
    public int CurrentHealth => currentHealth;

    [Header("Hit Reaction")]
    public float stunTime = 0.15f;
    public float hitFreezeVelocityTime = 0.06f;
    public bool isHitStunned = false;

    [Header("Knockdown")]
    public bool isKnockedDown = false;
    public float knockdownTime = 1.0f;
    public float knockdownSlowFactor = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private EnemyAttack enemyAttack; // 👈 NOVO

    public event Action<int, int> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        enemyAttack = GetComponent<EnemyAttack>(); // 👈 NOVO

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int dmg, Vector3 hitFromWorldPos, float knockbackForce, bool causesKnockdown = false)
    {
        currentHealth -= dmg;

        if (currentHealth < 0)
            currentHealth = 0;

        HitFlash flash = GetComponentInChildren<HitFlash>();
        if (flash != null)
            flash.Flash();

        if (DamageNumberSpawner.I != null)
            DamageNumberSpawner.I.Spawn(dmg, transform.position);

        EnemyAttack attack = GetComponent<EnemyAttack>();
        if (attack != null)
            attack.InterruptAttack();

        Vector2 dir = ((Vector2)transform.position - (Vector2)hitFromWorldPos).normalized;

        if (rb != null)
            rb.linearVelocity = dir * knockbackForce;

        // 🔥 NÃO INTERROMPE SE ESTIVER CAÍDO
        if (!isKnockedDown)
        {
            StopAllCoroutines();
        }

        if (causesKnockdown)
            StartCoroutine(KnockdownRoutine());
        else
            StartCoroutine(HitReactionRoutine());

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator HitReactionRoutine()
    {
        isHitStunned = true;

        yield return new WaitForSeconds(hitFreezeVelocityTime);

        if (rb != null)
            rb.linearVelocity *= 0.4f;

        yield return new WaitForSeconds(stunTime);

        isHitStunned = false;
    }

    IEnumerator KnockdownRoutine()
    {
        isKnockedDown = true;
        isHitStunned = true;

        if (sr != null)
            sr.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        yield return new WaitForSeconds(hitFreezeVelocityTime);

        if (rb != null)
            rb.linearVelocity *= knockdownSlowFactor;

        yield return new WaitForSeconds(knockdownTime);

        // 👇 LEVANTOU
        if (sr != null)
            sr.transform.rotation = Quaternion.identity;

        isKnockedDown = false;
        isHitStunned = false;

        // 🔥 RESET DO INIMIGO (ESSENCIAL)
        if (enemyAttack != null)
            enemyAttack.ResetForSpawn();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}