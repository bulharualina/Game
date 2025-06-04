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

        Debug.Log($"ResourcesHealthBar (Update): Reading from GlobalState -> Current: {currentHealth}, Max: {maxHealth}");
        if (maxHealth > 0)
        {
            _slider.maxValue = maxHealth; // Ensure slider max matches global max
            _slider.value = currentHealth; // Set slider value
            Debug.Log($"ResourcesHealthBar (Update): Slider set to -> Max: {_slider.maxValue}, Value: {_slider.value}, Fill Ratio: {_slider.value / _slider.maxValue}");

        }
        else
        {
            _slider.value = 0; // Set to 0 if max health is not valid
            Debug.LogWarning("ResourcesHealthBar (Update): GlobalState.resourceHealthMax is 0 or less. Slider value set to 0.");
        }
       
        

        
    }
}
