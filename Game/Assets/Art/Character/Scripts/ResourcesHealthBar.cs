using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesHealthBar : MonoBehaviour
{
    private Slider _slider;
   

  
    public GameObject globalState;


    private float currentHealth, maxHealth;



    // Start is called before the first frame update
    void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = globalState.GetComponent<GlobalState>().resourceHealth;
        maxHealth = globalState.GetComponent<GlobalState>().resourceHealthMax;
        if (maxHealth > 0)
        {
            _slider.maxValue = maxHealth; // Ensure slider max matches global max
            _slider.value = currentHealth; // Set slider value

            // Log the final slider values and ratio
            Debug.Log($"ResourcesHealthBar (Update): Slider updated -> Slider Max: {_slider.maxValue}, Slider Value: {_slider.value}, Fill Ratio: {_slider.value / _slider.maxValue}");

            // Optional: Update health text
            // if (healthText != null)
            // {
            //     healthText.text = $"{currentGlobalHealth}/{maxGlobalHealth}";
            // }
        }
        else
        {
            _slider.value = 0; // Set to 0 if max health is not valid
            Debug.LogWarning("ResourcesHealthBar (Update): GlobalState.resourceHealthMax is 0 or less. Slider value set to 0.");
        }
       
        

        
    }
}
