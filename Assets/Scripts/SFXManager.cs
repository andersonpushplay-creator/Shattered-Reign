using System.Collections;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioSource src;

    [Header("Combat SFX")]
    public AudioClip punch1;
    public AudioClip punch2;
    public AudioClip heavyImpact;
    public AudioClip playerHurt;
    public AudioClip enemyDeath;

    [Header("Parry SFX")]
    public AudioClip[] parryClangs;
    public AudioClip[] parryImpacts;

    [Header("Parry Settings")]
    [Range(0f, 0.1f)]
    public float parryImpactDelay = 0.02f;

    [Range(0.5f, 1.5f)]
    public float parryClangVolume = 1.0f;

    [Range(0.5f, 1.5f)]
    public float parryImpactVolume = 0.85f;

    [Header("Pitch Variation")]
    public bool randomizePitch = true;
    public float minPitch = 0.97f;
    public float maxPitch = 1.03f;

    void Awake()
    {
        if (src == null) src = GetComponent<AudioSource>();
        if (src == null) src = gameObject.AddComponent<AudioSource>();

        src.spatialBlend = 0f;
        src.playOnAwake = false;
        src.loop = false;
    }

    void Play(AudioClip clip)
    {
        if (clip == null || src == null) return;
        src.PlayOneShot(clip);
    }

    AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }

    IEnumerator PlayImpactDelayed(AudioClip clip)
    {
        yield return new WaitForSeconds(parryImpactDelay);

        if (clip == null || src == null) yield break;

        float originalPitch = src.pitch;

        if (randomizePitch)
            src.pitch = Random.Range(minPitch, maxPitch);
        else
            src.pitch = 1f;

        src.PlayOneShot(clip, parryImpactVolume);
        src.pitch = originalPitch;
    }

    public void PlayPunch1()
    {
        Play(punch1);
    }

    public void PlayPunch2()
    {
        Play(punch2);
    }

    public void PlayHeavyImpact()
    {
        Play(heavyImpact);
    }

    public void PlayPlayerHurt()
    {
        Play(playerHurt);
    }

    public void PlayEnemyDeath()
    {
        Play(enemyDeath);
    }

    public void PlayRandomPunch()
    {
        if (Random.value < 0.5f)
            PlayPunch1();
        else
            PlayPunch2();
    }

    public void PlayParry()
    {
        AudioClip clang = GetRandomClip(parryClangs);
        AudioClip impact = GetRandomClip(parryImpacts);

        float originalPitch = src.pitch;

        if (clang != null)
        {
            if (randomizePitch)
                src.pitch = Random.Range(minPitch, maxPitch);
            else
                src.pitch = 1f;

            src.PlayOneShot(clang, parryClangVolume);
        }

        src.pitch = originalPitch;

        if (impact != null)
        {
            StartCoroutine(PlayImpactDelayed(impact));
        }
    }
}