// C#
// HealthBar.cs
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Reference to the slider UI element
    public Slider slider;
    // Reference to a fill image to change color based on health
    public Image fillImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    // Set the max health value and update slider
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        UpdateHealthBarColor();
    }

    // Update the current health and slider value
    public void SetHealth(int health)
    {
        slider.value = health;
        UpdateHealthBarColor();
    }

    // Change health bar color based on current health
    private void UpdateHealthBarColor()
    {
        float healthPercent = slider.value / slider.maxValue;
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercent);
    }
}
