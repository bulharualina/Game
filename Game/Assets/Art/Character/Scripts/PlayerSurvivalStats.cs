using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSurvivalStats : MonoBehaviour
{
    public static PlayerSurvivalStats Instance { get; set; }



    //Calories
    public float currentCalories;
    public float maxCalories;
    public float damagePerSecondFromStarvation = 5f;

    float distanceTravelled = 0;
    Vector3 lastPosition;

    public GameObject player;

    //Hydration
    public float currentHydrationPercent;
    public float maxHydrationPercent;
    public float damagePerSecondFromDehydration = 8f;
    public bool isHydrationActive;

    
 


    [SerializeField] private Animator playerAnimator;
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
        
        currentCalories = maxCalories;
        currentHydrationPercent = maxHydrationPercent;
        StartCoroutine(decreaseHydrationHealth());
       

    }

    IEnumerator decreaseHydrationHealth() {
        while (true) 
        {
            currentHydrationPercent -= 2;
           
            yield return new WaitForSeconds(15); ;
        }
    
    }

    private void Update()
    {
        


        distanceTravelled += Vector3.Distance(player.transform.position,lastPosition);
        lastPosition = player.transform.position;

        if (distanceTravelled >= 10) 
        { 
            distanceTravelled = 0;
            currentCalories -= 1;
        }
       

    }
 

  
    public void setCalories(float newCalories) {
            currentCalories = newCalories;
    }
    public void setHydration(float newHydration) { 
    currentHydrationPercent = newHydration;
    }
}