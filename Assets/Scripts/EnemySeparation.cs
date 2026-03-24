using UnityEngine;

public class EnemySeparation : MonoBehaviour
{
    public float separationDistance = 1.5f;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Enemy")) return;

        Vector2 diff = transform.position - col.transform.position;

        if (diff.magnitude < separationDistance)
        {
            rb.AddForce(diff.normalized * 2f, ForceMode2D.Force);
        }
    }
}