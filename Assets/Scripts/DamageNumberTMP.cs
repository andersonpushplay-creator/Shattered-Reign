using TMPro;
using UnityEngine;

public class DamageNumberTMP : MonoBehaviour
{
    public float floatUpSpeed = 60f;
    public float lifeTime = 0.6f;
    public Vector2 randomOffset = new Vector2(12f, 8f);

    TextMeshProUGUI tmp;
    float t;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    public void Init(int dmg)
    {
        if (tmp != null) tmp.text = dmg.ToString();
    }

    void Update()
    {
        t += Time.deltaTime;

        // sobe
        transform.position += Vector3.up * floatUpSpeed * Time.deltaTime;

        // fade
        if (tmp != null)
        {
            var c = tmp.color;
            c.a = Mathf.Lerp(1f, 0f, t / lifeTime);
            tmp.color = c;
        }

        if (t >= lifeTime)
            Destroy(gameObject);
    }

    public static Vector3 AddRandomScreenOffset(Vector3 screenPos, Vector2 maxOffset)
    {
        screenPos.x += Random.Range(-maxOffset.x, maxOffset.x);
        screenPos.y += Random.Range(-maxOffset.y, maxOffset.y);
        return screenPos;
    }
}