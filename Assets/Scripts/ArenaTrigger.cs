using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    public WaveManager waveManager;

    bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entrou no trigger: " + other.name);

        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            Debug.Log("PLAYER DETECTADO - iniciando wave");

            if (waveManager != null)
                waveManager.StartWave();
        }
    }
}