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

    void Awake()
    {
        _slider = GetComponent<Slider>();
    }


    void Update()
    {
        currentCalories = playerSurvivalState.GetComponent<PlayerSurvivalStats>().currentCalories;
        maxCalories = playerSurvivalState.GetComponent<PlayerSurvivalStats>().maxCalories;

        float fillValue = currentCalories / maxCalories;
        _slider.value = fillValue;

        caloriesCounter.text = currentCalories + "/" + maxCalories;
    }
}
