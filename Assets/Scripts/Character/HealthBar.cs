using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    #region Data members

    private Slider _healthSlider;

    #endregion // Data members

    private void Awake()
    {
        _healthSlider = GetComponent<Slider>();
    }

    public void SetUpHealthBar(int maxHealth)
    {
        _healthSlider.minValue = 0;
        _healthSlider.maxValue = maxHealth;
    }

    public void UpdateHealth(int newHealth)
    {
        _healthSlider.value = newHealth;
    }
}