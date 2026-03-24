using UnityEngine;
using UnityEngine.UI;

public class Slider_UI_PlayerHP : MonoBehaviour
{
    public PlayerHealth target;
    public Slider slider;

    void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (target == null) target = FindFirstObjectByType<PlayerHealth>();
    }

    void OnEnable()
    {
        if (target != null)
            target.OnHealthChanged += UpdateBar;
    }

    void OnDisable()
    {
        if (target != null)
            target.OnHealthChanged -= UpdateBar;
    }

    void Start()
    {
        if (target != null && slider != null)
        {
            slider.maxValue = target.maxHealth;
            slider.value = target.CurrentHealth;
        }
    }

    void UpdateBar(int current, int max)
    {
        if (slider == null) return;
        slider.maxValue = max;
        slider.value = current;
    }
}