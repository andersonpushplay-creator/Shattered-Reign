using System.Collections.Generic;
using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    public bool knockdownHit = false;
    public bool finisherHit = false;
    public float laneTolerance = 0.6f;
    public int damage = 1;
    public float knockbackForce = 8f;

    SFXManager sfx;

    HashSet<EnemyHealth> hitEnemies = new HashSet<EnemyHealth>();

    void Awake()
    {
        sfx = FindFirstObjectByType<SFXManager>();
    }

    void OnEnable()
    {
        hitEnemies.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth eh = other.GetComponentInParent<EnemyHealth>();
        if (eh == null) return;

        if (hitEnemies.Contains(eh))
            return;

        float yDiff = Mathf.Abs(transform.position.y - eh.transform.position.y);
        if (yDiff > laneTolerance)
            return;

        hitEnemies.Add(eh);

        if (sfx != null)
            sfx.PlayRandomPunch();

        eh.TakeDamage(damage, transform.position, knockbackForce, knockdownHit);

        if (HitStopManager.I != null)
        {
            if (finisherHit)
                HitStopManager.I.Stop(0.09f);
            else
                HitStopManager.I.Stop(0.05f);
        }

        if (CameraShake.I != null)
        {
            if (finisherHit)
                CameraShake.I.Shake(0.12f, 0.2f);
            else
                CameraShake.I.Shake(0.08f, 0.15f);
        }
    }
}