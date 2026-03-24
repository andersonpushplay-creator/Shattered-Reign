using UnityEngine;

public class ParryTest : MonoBehaviour
{
    public SFXManager sfxManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Tecla P pressionada");

            if (sfxManager != null)
            {
                sfxManager.PlayParry();
                Debug.Log("PlayParry chamado");
            }
            else
            {
                Debug.LogError("SFXManager não foi atribuído no Inspector.");
            }
        }
    }
}