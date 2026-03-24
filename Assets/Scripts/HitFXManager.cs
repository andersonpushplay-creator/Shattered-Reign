using UnityEngine;

public class HitFXManager : MonoBehaviour
{
    public static HitFXManager Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject hitSparkPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SpawnHitFX(Vector2 worldPos, Vector2 direction)
    {
        if (hitSparkPrefab == null) return;

        // Rotaciona o cone do particle pra ir na direção do hit
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        var go = Instantiate(hitSparkPrefab, worldPos, Quaternion.Euler(0f, 0f, angle));
        // Se o prefab estiver com Stop Action = Destroy, ele se limpa sozinho.
    }
}