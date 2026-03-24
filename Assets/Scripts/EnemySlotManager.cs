using System.Collections.Generic;
using UnityEngine;

public class EnemySlotManager : MonoBehaviour
{
    public static EnemySlotManager I { get; private set; }

    [Header("Player")]
    public Transform player;                 // arraste o Player aqui (ou use tag Player)
    public string playerTag = "Player";

    [Header("Slots (offsets relativos ao player)")]
    public Vector2[] slotOffsets = new Vector2[]
    {
        new Vector2(-2.0f,  0.0f), // esquerda
        new Vector2( 2.0f,  0.0f), // direita
        new Vector2(-1.4f,  0.6f), // cima-esq
        new Vector2( 1.4f,  0.6f), // cima-dir
        new Vector2(-1.4f, -0.6f), // baixo-esq
        new Vector2( 1.4f, -0.6f), // baixo-dir
    };

    [Header("Lane snap")]
    public bool snapSlotToPlayerY = false;   // se quiser travar tudo na mesma linha do player

    int[] owner; // guarda instanceID do inimigo que ocupou o slot (0 = livre)

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        owner = new int[slotOffsets.Length];
    }

    void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
            else
            {
                var p2 = GameObject.Find("Player");
                if (p2 != null) player = p2.transform;
            }
        }
    }

    public int RequestSlot(GameObject enemy)
    {
        if (enemy == null) return -1;
        int id = enemy.GetInstanceID();

        // se ele já tem um slot, mantém
        for (int i = 0; i < owner.Length; i++)
            if (owner[i] == id) return i;

        // pega o primeiro livre
        for (int i = 0; i < owner.Length; i++)
        {
            if (owner[i] == 0)
            {
                owner[i] = id;
                return i;
            }
        }

        return -1; // sem vaga
    }

    public void ReleaseSlot(GameObject enemy)
    {
        if (enemy == null) return;
        int id = enemy.GetInstanceID();

        for (int i = 0; i < owner.Length; i++)
        {
            if (owner[i] == id)
                owner[i] = 0;
        }
    }

    public Vector2 GetSlotWorldPos(int slotIndex)
    {
        if (player == null) return Vector2.zero;
        if (slotIndex < 0 || slotIndex >= slotOffsets.Length) return player.position;

        Vector2 p = player.position;
        Vector2 w = p + slotOffsets[slotIndex];

        if (snapSlotToPlayerY)
            w.y = p.y;

        return w;
    }

    public bool IsMySlot(GameObject enemy, int slotIndex)
    {
        if (enemy == null) return false;
        if (slotIndex < 0 || slotIndex >= owner.Length) return false;
        return owner[slotIndex] == enemy.GetInstanceID();
    }
}