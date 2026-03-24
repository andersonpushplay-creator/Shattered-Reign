using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    public static EnemyAttackManager I;

    public int maxAttackers = 2;

    readonly List<GameObject> attackers = new List<GameObject>();

    void Awake()
    {
        I = this;
    }

    void CleanupNulls()
    {
        attackers.RemoveAll(a => a == null);
    }

    public bool CanAttack(GameObject enemy)
    {
        CleanupNulls();

        if (enemy == null) return false;

        if (attackers.Contains(enemy))
            return true;

        if (attackers.Count >= maxAttackers)
            return false;

        attackers.Add(enemy);
        return true;
    }

    public void StopAttack(GameObject enemy)
    {
        CleanupNulls();

        if (enemy == null) return;

        if (attackers.Contains(enemy))
            attackers.Remove(enemy);
    }

    public void ResetAll()
    {
        attackers.Clear();
    }
}