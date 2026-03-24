using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    public SpriteRenderer sr;
    public Color flashColor = new Color(1f, 0.2f, 0.2f, 1f);
    public float flashTime = 0.07f;

    Color originalColor;
    Coroutine routine;

    void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    public void Flash()
    {
        if (sr == null) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(DoFlash());
    }

    IEnumerator DoFlash()
    {
        sr.color = flashColor;
        yield return new WaitForSecondsRealtime(flashTime);
        sr.color = originalColor;
        routine = null;
    }
}