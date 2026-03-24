using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public bool movementLocked = false;
    public float speed = 2.5f;
    public float stopDistance = 0.2f;
    public float repathInterval = 0.5f;

    [Header("Combat Distance")]
    public float desiredAttackDistance = 1.0f;
    public float attackEngageBuffer = 0.35f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    EnemyHealth eh;
    EnemySlotAgent slotAgent;
    EnemyAttack attack;

    float repathTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        eh = GetComponent<EnemyHealth>();
        slotAgent = GetComponent<EnemySlotAgent>();
        attack = GetComponent<EnemyAttack>();
    }

    void FixedUpdate()
    {
        if (eh != null && eh.isKnockedDown)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (movementLocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (EnemySlotManager.I == null || EnemySlotManager.I.player == null || rb == null)
            return;

        if (eh != null && eh.isHitStunned)
            return;

        Transform player = EnemySlotManager.I.player;
        float distToPlayer = Vector2.Distance(rb.position, player.position);

        // 🔥 Se já entrou na zona de combate, aproxima pelo player, não pelo slot
        if (attack != null && distToPlayer <= attack.attackRange + attackEngageBuffer)
        {
            Vector2 toPlayer = (player.position - transform.position);
            Vector2 dir = toPlayer.normalized;

            if (distToPlayer > desiredAttackDistance)
                rb.linearVelocity = dir * speed;
            else
                rb.linearVelocity = Vector2.zero;
        }
        else
        {
            repathTimer -= Time.fixedDeltaTime;

            if (slotAgent != null && slotAgent.slotIndex < 0 && repathTimer <= 0f)
            {
                slotAgent.slotIndex = EnemySlotManager.I.RequestSlot(gameObject);
                repathTimer = repathInterval;
            }

            if (slotAgent == null || slotAgent.slotIndex < 0)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 targetPos = EnemySlotManager.I.GetSlotWorldPos(slotAgent.slotIndex);
            Vector2 pos = rb.position;
            Vector2 delta = targetPos - pos;

            if (delta.magnitude <= stopDistance)
                rb.linearVelocity = Vector2.zero;
            else
                rb.linearVelocity = delta.normalized * speed;
        }

        if (sr != null)
        {
            float xDiff = player.position.x - transform.position.x;
            if (xDiff > 0) sr.flipX = false;
            else if (xDiff < 0) sr.flipX = true;
        }
    }

    public void ForceReposition()
    {
        if (slotAgent != null && EnemySlotManager.I != null)
        {
            EnemySlotManager.I.ReleaseSlot(gameObject);
            slotAgent.slotIndex = -1;
            repathTimer = 0f;
        }
    }
}