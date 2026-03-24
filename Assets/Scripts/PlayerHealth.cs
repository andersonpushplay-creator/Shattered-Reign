using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHealth = 10;
    [SerializeField] int currentHealth;
    public int CurrentHealth => currentHealth;

    [Header("Hit Reaction")]
    public float stunTime = 0.25f;
    public float invulTime = 0.35f;
    public float knockbackForce = 8f;

    [HideInInspector] public bool isHitStunned;
    [HideInInspector] public bool blockMovement;

    public event Action<int, int> OnHealthChanged;

    Rigidbody2D rb;
    SpriteRenderer sr;
    SFXManager sfx;

    float stunTimer;
    float invulTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sfx = FindFirstObjectByType<SFXManager>();

        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Update()
    {
        // Stun
        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isHitStunned = false;
                blockMovement = false;
            }
        }

        // Invulnerável + piscando
        if (invulTimer > 0f)
        {
            invulTimer -= Time.deltaTime;

            if (sr != null)
                sr.enabled = (Mathf.FloorToInt(Time.time * 12f) % 2 == 0);

            if (invulTimer <= 0f && sr != null)
                sr.enabled = true;
        }
    }

    public void TakeDamage(int dmg, Vector2 hitFromWorldPos)
    {
        if (invulTimer > 0f) return;

        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        if (DamageNumberSpawner.I != null)
            DamageNumberSpawner.I.Spawn(dmg, hitFromWorldPos);

        GetComponent<HitFlash>()?.Flash();

        Vector2 hitDir = ((Vector2)transform.position - hitFromWorldPos).normalized;
        if (hitDir == Vector2.zero) hitDir = Vector2.right;

        HitFXManager.Instance?.SpawnHitFX(hitFromWorldPos, hitDir);

        Debug.Log($"Player tomou dano! HP: {currentHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // som de dano (não de tecla)
        sfx?.PlayPlayerHurt();

        // stun / trava input pra não matar knockback
        isHitStunned = true;
        blockMovement = true;
        stunTimer = stunTime;

        // invul
        invulTimer = invulTime;

        // knockback
        if (rb != null)
        {
            Vector2 dir = ((Vector2)transform.position - hitFromWorldPos).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
        {
            Debug.Log("PLAYER MORREU (placeholder).");
        }
    }

    public void HealFull()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}