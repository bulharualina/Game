using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CaloriesBar : MonoBehaviour
{
    private Slider _slider;
    public TextMeshProUGUI caloriesCounter;

    public GameObject playerSurvivalState;
   

    private float currentCalories, maxCalories;



    // Start is called before the first frame update
    void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentCalories = playerSurvivalState.GetComponent<PlayerSurvivalStats>().currentCalories;
        maxCalories = playerSurvivalState.GetComponent<PlayerSurvivalStats>().maxCalories;

        float fillValue = currentCalories / maxCalories;
        _slider.value = fillValue;

        caloriesCounter.text = currentCalories + "/" + maxCalories;
    }
}
