using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyHPBar : MonoBehaviour
{
    public EnemyHealth target;
    public Slider slider;

    void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (target == null) target = FindFirstObjectByType<EnemyHealth>();
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