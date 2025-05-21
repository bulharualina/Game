using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider _slider;
    public TextMeshProUGUI healthCounter;

    public GameObject playerSurvivalState;
    //private PlayerSurvivalStats playerStats;

    private float currentHealth, maxHealth;



    // Start is called before the first frame update
    void Awake()
    {
        _slider=GetComponent<Slider>();   
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = playerSurvivalState.GetComponent<PlayerSurvivalStats>().currentHealth;
        maxHealth = playerSurvivalState.GetComponent<PlayerSurvivalStats>().maxHealth;

        float fillValue = currentHealth/maxHealth;
        _slider.value = fillValue;

        healthCounter.text = currentHealth+ "/" +maxHealth;
    }
}
