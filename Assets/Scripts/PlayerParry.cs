using System.Collections;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    [Header("Parry")]
    public KeyCode parryKey = KeyCode.K;
    public float parryWindow = 0.25f;
    public bool counterWindow = false;
    public bool isParrying = false;

    SpriteRenderer sr;
    Color originalColor;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    void Update()
    {
        if (Input.GetKeyDown(parryKey))
        {
            StartCoroutine(ParryRoutine());
        }
    }

    IEnumerator ParryRoutine()
    {
        isParrying = true;

        if (sr != null)
            sr.color = Color.cyan;

        Debug.Log("Parry ativo");

        yield return new WaitForSeconds(parryWindow);

        isParrying = false;

        if (sr != null)
            sr.color = originalColor;
    }
}