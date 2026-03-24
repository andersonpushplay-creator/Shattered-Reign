using UnityEngine;

public class EnemyHitboxDamage : MonoBehaviour
{
    public int damage = 1;

    EnemyAttack attack;

    void Awake()
    {
        attack = GetComponentInParent<EnemyAttack>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerParry parry = other.GetComponentInParent<PlayerParry>();
        PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();

        if (ph == null) return;

        // se o player estiver em parry, cancela o golpe
        if (parry != null && parry.isParrying)  
        {
            Debug.Log("PARRY!");

            parry.counterWindow = true;

            if (attack != null)
                attack.OnParried();

            if (HitStopManager.I != null)
                HitStopManager.I.Stop(0.06f);

            if (CameraShake.I != null)
                CameraShake.I.Shake(0.08f, 0.12f);

            return;
        }

        Debug.Log("ACERTOU PLAYER!");
        ph.TakeDamage(damage, transform.position);

        if (attack != null)
            attack.NotifyHitConnected();
    }
}