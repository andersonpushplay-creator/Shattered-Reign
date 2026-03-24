using UnityEngine;

public class EnemySlotAgent : MonoBehaviour
{
    public int slotIndex = -1;

    void Start()
    {
        if (EnemySlotManager.I != null)
            slotIndex = EnemySlotManager.I.RequestSlot(gameObject);
    }

    void OnDisable()
    {
        if (EnemySlotManager.I != null)
            EnemySlotManager.I.ReleaseSlot(gameObject);
    }

    void OnDestroy()
    {
        if (EnemySlotManager.I != null)
            EnemySlotManager.I.ReleaseSlot(gameObject);
    }
}