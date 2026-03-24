using UnityEngine;

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner I;

    [Header("Refs")]
    public Canvas worldCanvas;                // arrasta WorldCanvas aqui
    public DamageNumberTMP damagePrefab;      // arrasta o prefab DamageNumber aqui
    public Camera cam;                        // pode deixar vazio (pega a MainCamera)

    void Awake()
    {
        I = this;
        if (cam == null) cam = Camera.main;
    }

    public void Spawn(int dmg, Vector3 worldPos)
    {
        if (worldCanvas == null || damagePrefab == null || cam == null) return;

        Vector3 screen = cam.WorldToScreenPoint(worldPos);
        screen = DamageNumberTMP.AddRandomScreenOffset(screen, damagePrefab.randomOffset);

        var inst = Instantiate(damagePrefab, worldCanvas.transform);
        inst.transform.position = screen;
        inst.Init(dmg);
    }
}