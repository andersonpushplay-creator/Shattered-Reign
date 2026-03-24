using UnityEngine;

public class PlayerController_BeatEmUp : MonoBehaviour
{
    public int FacingDir { get; private set; } = 1; // 1 = direita, -1 = esquerda
    [SerializeField] private SpriteRenderer sr;

    public float moveSpeed = 6f;

    Rigidbody2D rb;
    PlayerHealth ph;

    void Awake()
    {
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        ph = GetComponent<PlayerHealth>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // trava só quando tomou hit (pra knockback não ser zerado)
        if (ph != null && ph.blockMovement)
            return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (x > 0.01f) FacingDir = 1;
        else if (x < -0.01f) FacingDir = -1;

        if (sr != null) sr.flipX = (FacingDir == -1);

        Vector2 input = new Vector2(x, y);
        if (input.sqrMagnitude > 1f) input = input.normalized;

        rb.linearVelocity = input * moveSpeed;
    }
}