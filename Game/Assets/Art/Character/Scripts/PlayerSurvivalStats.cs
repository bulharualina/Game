using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSurvivalStats : MonoBehaviour
{
    public static PlayerSurvivalStats Instance { get; set; }

    //Health
    public float currentHealth;
    public float maxHealth;

    //Calories
    public float currentCalories;
    public float maxCalories;

    float distanceTravelled = 0;
    Vector3 lastPosition;

    public GameObject player;

    //Hydration
    public float currentHydrationPercent;
    public float maxHydrationPercent;

    public bool isHydrationActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentCalories = maxCalories;
        currentHydrationPercent = maxHydrationPercent;
        StartCoroutine(decreaseHydration());
    }

    IEnumerator decreaseHydration() {
        while (true) 
        {
            currentHydrationPercent -= 1;
            yield return new WaitForSeconds(15); ;
        }
    
    }

    private void Update()
    {
        distanceTravelled += Vector3.Distance(player.transform.position,lastPosition);
        lastPosition = player.transform.position;

        if (distanceTravelled >= 15) 
        { 
            distanceTravelled = 0;
            currentCalories -= 1;
        }


        //Testing
        if (Input.GetKeyDown(KeyCode.N)) {
            currentHealth -= 10;
        }
    }

    public void setHealth(float newHealth) { 
        currentHealth = newHealth;
    }
    public void setCalories(float newCalories) {
            currentCalories = newCalories;
    }
    public void setHydration(float newHydration) { 
    currentHydrationPercent = newHydration;
    }
}