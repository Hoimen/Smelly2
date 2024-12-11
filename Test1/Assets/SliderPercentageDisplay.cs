using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderPercentageDisplay : MonoBehaviour
{
    // Reference to the Slider component
    public Slider slider;
    // Reference to the TextMeshPro component
    public TextMeshProUGUI percentageText;

    void Start()
    {
        // Update the text initially and add a listener to update it whenever the slider value changes
        UpdatePercentageText();
        slider.onValueChanged.AddListener(delegate { UpdatePercentageText(); });
    }

    // Update the percentage text
    void UpdatePercentageText()
    {
        // Convert slider value to percentage
        int percentage = Mathf.RoundToInt(slider.value * 100);
        // Display percentage on TextMeshPro in "XX%" format
        percentageText.text = percentage.ToString() + "";
    }

    void OnDestroy()
    {
        // Remove listener when the script or object is destroyed
        slider.onValueChanged.RemoveListener(delegate { UpdatePercentageText(); });
    }
}
