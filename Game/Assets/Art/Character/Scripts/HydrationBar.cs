using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HydrationBar : MonoBehaviour
{
    private Slider _slider;
    public TextMeshProUGUI hydrationCounter;

    public GameObject playerSurvivalState;
    

    private float currentHydration, maxHydration;




    void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentHydration = playerSurvivalState.GetComponent<PlayerSurvivalStats>().currentHydrationPercent;
        maxHydration = playerSurvivalState.GetComponent<PlayerSurvivalStats>().maxHydrationPercent;

        float fillValue = currentHydration / maxHydration;
        _slider.value = fillValue;

        hydrationCounter.text = currentHydration + "%";
    }
}
